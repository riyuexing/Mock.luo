﻿using Autofac;
using AutoMapper;
using Mock.Code;
using Mock.Code.Helper;
using Mock.Data;
using Mock.Data.Models;
using Mock.Domain;
using Mock.Luo.Areas.Plat.Models;
using Mock.Luo.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Mock.Luo.Areas.Plat.Controllers
{
    public class ArticleController : CrudController<Article, ArticleViewModel>
    {
        // GET: Plat/Article

        IArticleRepository _service;
        IItemsDetailRepository _itemsDetailRepository;
        public ArticleController(IArticleRepository service, IItemsDetailRepository itemsDetailRepository, IComponentContext container) : base(container)
        {
            this._service = service;
            this._itemsDetailRepository = itemsDetailRepository;
        }

        public override ActionResult Form(int Id)
        {
            //取出文章对应的多个标签Id
            var tagActive = _service.IQueryable(u => u.DeleteMark == false && u.Id == Id)
                .Select(u => u.TagArts.Select(r => r.TagId)).FirstOrDefault();

            ViewBag.TagActive = JsonHelper.SerializeObject(tagActive);

            return base.Form(Id);
        }
        public ActionResult GetDataGrid(Pagination pag, string search = "")
        {
            return Content(_service.GetDataGrid(pag, search).ToJson());
        }

        /// <summary>
        /// 最新文章
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRecentArticle()
        {
            return Result(_service.GetRecentArticle(5));
        }

        /// <summary>
        /// 得到博客列表页面
        /// </summary>
        /// <param name="pag"></param>
        /// <returns></returns>
        [Skip]
        public ActionResult GetIndexGird(Pagination pag, string category = "", string tag = "", string archive = "")
        {
            if (category.IsNullOrEmpty() && tag.IsNullOrEmpty() && archive.IsNullOrEmpty())
            {
                throw new ArgumentNullException("参数异常!!");
            }
            if (pag.sort.IsNullOrEmpty())
            {
                pag.sort = "Id";
            }
            if (pag.order.IsNullOrEmpty())
            {
                pag.order = "desc";
            }
            DataGrid dg = _service.GetCategoryTagGrid(pag, category, tag, archive);
            return Content(dg.ToJson());
        }

        /// <summary>
        /// 文章新增，编辑，由于后期加了一个文章可以有多个标签，一个标签可对应多个文章，就是多对多的关系，
        /// 所以，这里就需要重新对数据进行处理后，提交数据库
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override ActionResult Edit(ArticleViewModel viewModel, int id = 0)
        {
            if (!ModelState.IsValid)
            {
                return Error(ModelState.Values.Where(u => u.Errors.Count > 0).FirstOrDefault().Errors[0].ErrorMessage);
            }

            string TagIds = Request["Tag"].ToString();
            List<TagArt> tagArtList = new List<TagArt> { };
            if (TagIds.IsNotNullOrEmpty())
            {
                foreach (var i in TagIds.Split(',').Select(u => Convert.ToInt32(u)).ToList())
                {
                    tagArtList.Add(new TagArt
                    {
                        TagId = i,
                        AId = id
                    });
                }
            }
            Article entity;
            if (id == 0)
            {
                entity = Mapper.Map<ArticleViewModel, Article>(viewModel);
                entity.Create();
                entity.TagArts = tagArtList;
                entity.Archive = DateTime.Now.ToString("yyy年MM月");
                _service.Insert(entity);
            }
            else
            {
                var tEntityModel = _service.FindEntity(id);

                if (tEntityModel == null)
                    return Error($"Id为{id}未找到任何类型为{viewModel.GetType().Name}的实体对象");

                entity = Mapper.Map(viewModel, tEntityModel);

                entity.Modify(id);
                using (var db = new RepositoryBase().BeginTrans())
                {
                    db.Update(entity);
                    db.Delete<TagArt>(u => u.AId == id);
                    db.Insert(tagArtList);
                    db.Commit();
                }
            }
            //要根据新增或修改的文章类型，来判断是否删除缓存中的数据
            string itemCode = entity.ItemsDetail.ItemCode;
            if (itemCode.Equals(CategoryCode.justfun))
            {
                _redisHelper.KeyDeleteAsync(string.Format(ConstHelper.App, "JustFun"));
            }
            if (itemCode.Equals(CategoryCode.feelinglife))
            {
                _redisHelper.KeyDeleteAsync(string.Format(ConstHelper.App, "FellLife"));
            }

            return Success();
        }
    }
}
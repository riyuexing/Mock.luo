﻿using Autofac;
using Mock.Data;
using Mock.Data.Models;
using Mock.Domain;
using Mock.Luo.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mock.Luo.Areas.Plat.Controllers
{
    public class PointArticleController : BaseController
    {
        // GET: Plat/PointArticle

        private readonly IPointArticleRepository _service;
        public PointArticleController(IPointArticleRepository service)
        {
            this._service = service;
        }

        //根据文章id得到点赞人信息
        public ActionResult GetDataGrid(int AId)
        {
            return Result(_service.GetDataGrid(AId));
        }
        //用户点赞
        public ActionResult Edit(PointArticle entry)
        {
            entry.AddTime = DateTime.Now;
            entry.UserId = (int)OperatorProvider.Provider.CurrentUser.UserId;

            var pointCount = _service.IQueryable(u => u.UserId == entry.UserId && u.AId == entry.AId).Count();
            //点当前文章已经被点赞时，用户再次点赞，则删除之前的赞
            if (pointCount > 0)
            {
                _service.Delete(u => u.UserId == entry.UserId && u.AId == entry.AId);
            }
            else
            {
                _service.Insert(entry);
            }

            return Success();
        }

    }
}
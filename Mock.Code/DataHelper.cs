﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Mock.Code
{
    public class DataHelper
    {

        #region  把对象转换成JSON格式
        //js序列化器
        static JavaScriptSerializer jss = new JavaScriptSerializer();
        //日期序列化模版
        static IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        /// <summary>
        /// 把对象转换成JSON格式
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>json格式数据</returns>
        public static string ObjToJson(object obj)
        {

            return jss.Serialize(obj);
        }

        //序列化成固定日期格式的JSON数据
        public static string ObjToJsonFormatDate(object obj)
        {
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd";
            return JsonConvert.SerializeObject(obj, Formatting.Indented, timeConverter);
        }
        #endregion

        //反序列化成对象
        public static T JsonToObj<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        #region 把一个字符串进行MD5加密
        public static string Md5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, FormsAuthPasswordFormat.MD5.ToString());
        }
        #endregion


        public static Dictionary<string, object> GetAddOrEditOrDel<T>(HttpRequestBase Request)
        {
            List<T> insertedList, deletedList, updatedList;
            string Inserted = Request["Inserted"] ?? "[]";
            insertedList = jss.Deserialize<List<T>>(Inserted);

            string Deleted = Request["Deleted"] ?? "[]";
            deletedList = jss.Deserialize<List<T>>(Deleted);

            string Updated = Request["Updated"] ?? "[]";
            updatedList = jss.Deserialize<List<T>>(Updated);

            Dictionary<string, object> di = new Dictionary<string, object>();
            di.Add("Inserted", insertedList);
            di.Add("Deleted", deletedList);
            di.Add("Updated", updatedList);
            return di;
        }
     


    }
}
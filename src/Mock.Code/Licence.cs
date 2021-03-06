﻿using System.Configuration;
using System.Web;
using Mock.Code.Security;

namespace Mock.Code
{
    public sealed class Licence
    {
        public static bool IsLicence(string key)
        {
            string host = HttpContext.Current.Request.Url.Host.ToLower();
            if (host.Equals("localhost"))
                return true;
            string licence = ConfigurationManager.AppSettings["LicenceKey"];
            if (licence != null && licence == Md5Helper.Md5(key, 32))
                return true;

            return false;
        }
        public static string GetLicence()
        {
            var licence = Configs.Configs.GetValue("LicenceKey");
            if (string.IsNullOrEmpty(licence))
            {
                licence = Utils.GuId();
                Configs.Configs.SetValue("LicenceKey", licence);
            }
            return Md5Helper.Md5(licence, 32);
        }
    }
}

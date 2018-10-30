﻿using Mock.Code.Log;
using Mock.Code.Web;
using Mock.Data.AppModel;

namespace Mock.Domain.Interface
{
    public interface ILogInfoRepository 
    {
        
        /// <summary>
        /// 根据条件得到日志的分页数据
        /// </summary>
        /// <param name="pag">分页条件</param>
        /// <returns>DataGrid实体</returns>
        DataGrid GetDataGrid(PageDto pag, string search);

        void LogError(LogMessage logEntity, string logName);
    }
}

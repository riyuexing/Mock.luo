﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mock.Data
{
    /// <summary>
    /// easyuidatagrid模型
    /// 用来返回EasyUI的DataGrid值
    /// </summary>
    public class DataGrid
    {
        public int total { get; set; }
        public object rows { get; set; }
        public object footer { get; set; }
    }
}

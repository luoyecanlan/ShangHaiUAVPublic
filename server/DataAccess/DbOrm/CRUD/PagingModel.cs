using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.CRUD
{
    public class PagingModel<T>
    {
        public PagingModel()
        {

        }

        public PagingModel(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
        /// <summary>
        /// 单页数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 总记录条数
        /// </summary>
        public int SnumSize { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable<T> Data { get; set; }
    }
}

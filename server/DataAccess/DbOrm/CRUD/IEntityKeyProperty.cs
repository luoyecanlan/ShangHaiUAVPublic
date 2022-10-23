using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.CRUD
{
    /// <summary>
    /// 实体主键属性接口
    /// </summary>
    public interface IEntityKeyProperty
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        int Id { get; set; }
    }
}

using AntiUAV.Bussiness.Service;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.ServiceImpl
{
    public class WhiteListService : MetadataService<WhiteListInfo, WhiteListUpdate, WhiteListDel, WhiteListAdd>, IWhiteListService
    {
        public WhiteListService(IEntityCrudService orm) : base(orm)
        {
        }
    }
}

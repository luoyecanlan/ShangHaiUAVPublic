using AntiUAV.Bussiness.Service;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.ServiceImpl
{
    public class MapConfigService : MetadataService<MapConfigInfo,MapConfigUpdate,MapConfigDel,MapConfigAdd>,IMapConfigService
    {
        public MapConfigService(IEntityCrudService orm) : base(orm)
        {
        }
    }
}

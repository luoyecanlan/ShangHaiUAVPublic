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
    public class TargetTurnService : MetadataService<TargetTurnInfo, TargetTurnUpdate, TargetTurnDel, TargetTurnAdd>,ITargetTurnService
    {
        public TargetTurnService(IEntityCrudService orm) : base(orm)
        {
        }
    }
}

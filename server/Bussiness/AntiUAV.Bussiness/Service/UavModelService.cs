using AntiUAV.Bussiness.ServiceImpl;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Service
{
    public class UavModelService : MetadataService<UAVModelInfo, UAVModelUpdate, UAVModelDel, UAVModelAdd>, IUavModelService
    {
        public UavModelService(IEntityCrudService orm) : base(orm)
        {
        }
    }
}

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
    public class PersonalizationService : MetadataService<PersonalizationInfo, PersonalizationUpdate, PersonalizationDel, PersonalizationAdd>, IPersonalizationService
    {
        public PersonalizationService(IEntityCrudService orm) : base(orm)
        {
        }
    }
}

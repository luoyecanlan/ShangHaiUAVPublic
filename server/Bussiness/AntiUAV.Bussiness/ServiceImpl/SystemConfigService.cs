using AntiUAV.Bussiness.Service;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AntiUAV.Bussiness.ServiceImpl
{
    public class SystemConfigService : MetadataService<SysConfig, SysConfig, SysConfigDel, SysConfigAdd>, ISystemConfigService
    {
        public SystemConfigService(IEntityCrudService orm) : base(orm)
        {
        }

        public async Task<SysConfig> GetConfigAsync(string name)
        {
            var data = await _orm.GetAnyAsync<SysConfig>();
            return data?.FirstOrDefault(f => f.Name == name);
        }
    }
}

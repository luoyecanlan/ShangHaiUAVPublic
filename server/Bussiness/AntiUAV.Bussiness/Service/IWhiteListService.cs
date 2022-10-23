using DbOrm.AntiUAV.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AntiUAV.Bussiness.Service
{
    public interface IWhiteListService : IMetadataService<WhiteListInfo, WhiteListUpdate, WhiteListDel, WhiteListAdd>
    {
    }
}

using AntiUAV.DeviceServer.Abstractions.HostService.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using AntiUAV.DeviceServer.Abstractions.HostService.Cmds;

namespace AntiUAV.DeviceServer.Abstractions.HostService.ServiceImpl
{
    public class PeerRouteBase : IPeerRoute
    {
        public PeerRouteBase(IMemoryCache memory)
        {
            _memory = memory;
        }

        private readonly IMemoryCache _memory;


        #region IPeerRoute

        /// <summary>
        /// 装载IPeerSysCmd，根据cmd_sys_PeerSysCmdType，填充到内存
        /// </summary>
        /// <param name="provider"></param>
        public void LoadCmds(IServiceProvider provider)
        {
            int category = _memory.GetDevice().Category;
            if (category <= 10000)
                throw new Exception("invalid device category , cmd load fail.");

            var sys_cmds = provider?.GetServices<IPeerSysCmd>().Where(x => x.Key == category.ToString()).ToList();
            //所有系统类型 PeerSysCmdType 都遍历一遍，没有就给默认
            var sc_type = typeof(PeerSysCmdType);
            foreach (PeerSysCmdType order in Enum.GetValues(sc_type))
            {
                var fieldName = Enum.GetName(sc_type, order);
                var attr = sc_type.GetField(fieldName).GetCustomAttributes(typeof(PeerCmdAttribute), true)?.FirstOrDefault() as PeerCmdAttribute ??
                    order.GetType().GetCustomAttributes(typeof(PeerCmdAttribute), true)?.FirstOrDefault() as PeerCmdAttribute;
                IPeerSysCmd cmd = null;
                if (attr.Only)
                {
                    cmd = sys_cmds.SingleOrDefault(x => x.Order == order);//获取命令
                }
                else
                {
                    cmd = sys_cmds.FirstOrDefault(x => x.Order == order);//获取命令
                }
                if (attr.Must == true && cmd == null)
                    throw new Exception($"cmd '{order}' must exist.");
                _memory.Set($"cmd_sys_{order}", cmd ?? new SysDefaultCmd(order));//不是必须存在的，但是没存在，则给个默认实现
            }

            var act_cmds = provider?.GetServices<IPeerCmd>().Where(x => x.Category == category);
            //所有命令类型 PeerCmdType 都便利一遍，没有就给默认
            var c_type = typeof(PeerCmdType);
            foreach (PeerCmdType order in Enum.GetValues(c_type))
            {
                var cmds = act_cmds.Where(x => x.Order == order).ToList() ?? new List<IPeerCmd>();//获取命令
                _memory.Set($"cmd_act_{order}", cmds);
            }
        }


        //private bool GetSysCmdFlag(PeerSysCmdType cmdType) => _memory.TryGetValue($"flag_{cmdType}_invad", out _);

        //private void SetSysCmdFlag(PeerSysCmdType cmdType) => _memory.Set($"flag_{cmdType}", cmdType);

        /// <summary>
        /// 获取cmd_sys_PeerSysCmdType内存中的IPeerSysCmd
        /// </summary>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public virtual IPeerSysCmd GetSysCmd(PeerSysCmdType cmdType)
        {
            _memory.TryGetValue($"cmd_sys_{cmdType}", out IPeerSysCmd cmd);
            return cmd;
            //_memory.GetOrCreate(cmdType, entrty =>
            //{
            //    var category = _memory.GetDevice()?.Category;
            //    return _provider?.GetServices<IPeerSysCmd>()?.FirstOrDefault(x => x.Order == cmdType && x.Category == category);
            //});
        }

        /// <summary>
        /// 获取cmd_act_PeerCmdType内存中的IPeerCmd列表
        /// </summary>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        public virtual IEnumerable<IPeerCmd> GetCmd(PeerCmdType cmdType)
        {
            _memory.TryGetValue($"cmd_act_{cmdType}", out IEnumerable<IPeerCmd> cmd);
            return cmd;
            //_memory.GetOrCreate(cmdType, entrty =>
            //{
            //    var category = _memory.GetDevice()?.Category;
            //    return _provider?.GetServices<IPeerCmd>()?.Where(x => x.Order == cmdType && x.Category == category).ToList() ?? new List<IPeerCmd>();
            //});
        }

        /// <summary>
        /// 根据key获取内存中第一个IPeerCmd
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual IPeerCmd GetCmd(string key) => GetCmd(PeerCmdType.Action)?.FirstOrDefault(x => x.Key == key);

        /// <summary>
        /// 异步执行IPeerContent中解析到的PeerSysCmdType类型命令
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<bool?> ExecuteSysCmdAsync(PeerSysCmdType cmdType, IPeerContent content)
        {
            var cmd = GetSysCmd(cmdType);
            if (cmd != null)
            {
                if (!await cmd.Invoke(content))
                {
                    content.Error = new PeerException($"recive data {cmdType} failure.");
                    return false;
                }
                return true;
            }
            return null;
            //var cmd = GetSysCmd(cmdType);
            //if (cmd == null && !GetSysCmdFlag(cmdType))
            //{
            //    content.Error = new PeerException($"recive data but not find {cmdType} func.");//为了输出方便这么写下
            //    GetSysCmd(PeerSysCmdType.Warn)?.Invoke(content);
            //    SetSysCmdFlag(cmdType);
            //    content.Error = null;//清理错误信息
            //    return null;
            //}
            //else
            //{
            //    if (!await cmd.Invoke(content))
            //    {
            //        content.Error = new PeerException($"recive data {cmdType} failure.");
            //        return false;
            //    }
            //    return true;
            //}
        }

        /// <summary>
        /// 解析所有命令，将通讯数据映射到系统
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task ExcutePipeLineAsync(IPeerContent content)
        {
            //这个地方有待修改下更好的样子，
            if (content == null) throw new PeerException("pipe line content is null.");

            if (CheckGuidance(content.Source))
            {
                content.Route = "BEGD";
            }
            else
            {
                #region 包体校验
                //头尾校验
                content.IsHeadAndTail = await ExecuteSysCmdAsync(PeerSysCmdType.CheckHeadAndTail, content);
                if (content.ForcedOver) return;
                //校验和校验
                content.IsChecksnum = await ExecuteSysCmdAsync(PeerSysCmdType.Checksnum, content);
                if (content.ForcedOver) return;

                GetSysCmd(PeerSysCmdType.Out)?.Invoke(content);//执行透传
                #endregion
            }

            #region 过滤器
            foreach (var filter in GetCmd(PeerCmdType.Filter))
            {
                await filter?.Invoke(content);
                if (content.ForcedOver) return;
            }
            #endregion

            #region 数据解析
            var route = GetSysCmd(PeerSysCmdType.Route);
            if (route == null)
            {
                content.Error = new PeerException("route cmd is null.");
                return;
            }
            if (await route?.Invoke(content))//route_cmd执行
            {
                var cmd = GetCmd(content.Route);
                if (cmd != null)
                {
                    await cmd.Invoke(content);//track_cmd执行 gd_cmd执行
                }
            }
            if (content.ForcedOver) return;
            #endregion

            #region 中间件
            foreach (var middleware in GetCmd(PeerCmdType.Middleware))
            {
                middleware?.Invoke(content);//guidance_cmd turn_cmd执行（中间件）
                if (content.ForcedOver) return;
            }
            #endregion
        }
        #endregion

        private bool CheckGuidance(byte[] buff)
        {
            return Encoding.UTF8.GetString(buff, 0, 4) == "BEGD";
        }
    }
}

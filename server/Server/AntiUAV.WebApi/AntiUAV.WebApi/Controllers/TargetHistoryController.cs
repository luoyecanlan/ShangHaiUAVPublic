using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AntiUAV.Bussiness;
using AntiUAV.Bussiness.Models;
using AntiUAV.Bussiness.Service;
using AntiUAV.WebApi.Model;
using AntiUAV.WebApi.SwaggerDoc;
using DbOrm.AntiUAV.Entity;
using DbOrm.CRUD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace AntiUAV.WebApi.Controllers
{
    /// <summary>
    /// 目标历史数据管理
    /// </summary>
    [Route("api/his")]
    [ApiController]
    [ApiGroup(ApiGroupNames.TargetHistory)]
    [Authorize(Roles = SystemRole.Client)]
    public class TargetHistoryController : ControllerBase
    {
        IHistoryTargetService _history;
        IHistoryTrackService _track;
        ISummaryService _summary;
        IUavModelService _uav;
        ILogger<TargetHistoryController> _logger;
        public TargetHistoryController(IHistoryTargetService history, IHistoryTrackService track,
            ISummaryService summary, IUavModelService uav, ILogger<TargetHistoryController> logger)
        {
            _history = history;
            _track = track;
            _summary = summary;
            _uav = uav;
            _logger = logger;
        }

        /// <summary>
        /// 获取历史目标信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        [HttpPost("target")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<PagingModel<HistoryTgInfo>>), 200)]
        public async Task<IActionResult> GetHistoryAsync([FromQuery] int index, [FromQuery] int size, [FromBody] SeachConditions conditions)
        {
            if (conditions == null)
            {
                return Ok(await _history.GetAnyAsync<HistoryTgInfo>(size, index, null, f => f.Endtime, true));
            }
            else
            {
                return Ok(await _history.GetHisAnyAsync(size, index, conditions));
            }
        }
        /// <summary>
        /// 熱力圖 前一星期无人机统计
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        [HttpPost("heatMapData")]
        [Authorize(Roles = SystemRole.Admin)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<PagingModel<HistoryTgInfo>>), 200)]
        public async Task<IActionResult> HeatMapAsync()
        {

            SeachConditions conditions = new SeachConditions();
            conditions.Start = DateTime.Now.AddDays(-7);
            conditions.End = DateTime.Now;
            var track = (await _history.GetHeatMapHisIdsAsync(conditions));
            List<TrackList> result = new List<TrackList>();

            foreach (var temp in track)
            {
                TrackList trackList = new TrackList()
                {
                    point = new Point() { lat = temp.Lat.ToString(), lng = temp.Lng.ToString() },
                    address = "test"
                };
                result.Add(trackList);
            }
            return Ok(result);
        }
        /// <summary>
        /// 河流图 统计前一个月
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        [HttpPost("riverData")]
        [Authorize(Roles = SystemRole.Admin)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<PagingModel<HistoryTgInfo>>), 200)]
        public async Task<IActionResult> riverAsync()
        {

            SeachConditions conditions = new SeachConditions();
            conditions.Start = DateTime.Now.AddDays(-30);
            conditions.End = DateTime.Now;
            var list = await _history.GetRiverAsync(conditions);
            var t = list.GroupBy(a => a.Starttime.Date);
            var result = new List<List<string>>();
            foreach (IGrouping<DateTime, HistoryTargetInfo> item in t)
            {
                string date = item.Key.Date.ToString().Split(' ')[0];
                var tmp = new List<string>();
                string positionName = "";
                foreach (var his in item)
                {

                    string devId = his.TgId.Split('.')[1];

                    switch (devId)
                    {
                        case "1": positionName = "中银"; break;
                        case "2": positionName = "世博园"; break;
                        case "3": positionName = "文化公园"; break;
                    }

                }
                tmp.Add(date);

                tmp.Add(item.Count().ToString());
                tmp.Add(positionName);
                result.Add(tmp);

            }
            List<List<string>> results1 = new List<List<string>>();
            List<List<string>> results2 = new List<List<string>>();
            foreach (var tmp in result)
            {
                List<string> lists = new List<string>();
                lists.Add(tmp[0]);

                lists.Add((Convert.ToInt32(tmp[1]) + 30).ToString());
                lists.Add("中银");

                results1.Add(lists);

                List<string> lists1 = new List<string>();

                lists1.Add(tmp[0]);
                lists1.Add((Convert.ToInt32(tmp[1]) + 10).ToString());
                lists1.Add("世博园");

                results2.Add(lists1);
            }
            result.AddRange(results1);
            result.AddRange(results2);
            return Ok(result);
        }
        /// <summary>
        /// 仪表盘 当天统计
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        [HttpPost("weiguiData")]
        [Authorize(Roles = SystemRole.Admin)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<PagingModel<HistoryTgInfo>>), 200)]
        public async Task<IActionResult> weiguiDataAsync()
        {

            SeachConditions conditions = new SeachConditions();
            //conditions.Start = DateTime.Now.Date;
            conditions.Start = DateTime.Now.AddDays(-3);
            conditions.End = DateTime.Now;
            var targets = (await _history.GetRiverAsync(conditions));
            var t1 = new Weigui(); t1.name = "中银";
            var t2 = new Weigui(); t2.name = "世博园";
            var t3 = new Weigui(); t3.name = "文化公园";
            foreach (var temp in targets)
            {
                //解析tgid，根据探测设备id判断位置，可能改变需要注意。
                string name = "";
                switch (temp.TgId.Split('.')[1])
                {
                    case "1":  t1.score = t1.score + 1;t1.value = t1.value + 1; break;
                    case "2":  t2.score = t2.score + 1; t2.value = t2.value + 1; break;
                    case "3":  t3.score = t3.score + 1; t3.value = t3.value + 1; break;
                }
            }
            List<Weigui> result = new List<Weigui>();
            result.Add(t1);
            result.Add(t2);
            result.Add(t3);
            return Ok(result);
        }



        /// <summary>
        /// 获取历史目标信息
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        [HttpPost("target/ids")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<IEnumerable<int>>), 200)]
        public async Task<IActionResult> GetHistoryIdsAsync([FromBody] SeachConditions conditions)
        {
            return Ok(await _history.GetHisAnyIdsAsync(conditions));
        }

        /// <summary>
        /// 获取所有飞机类型ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("uavmodel")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<IEnumerable<UAVModelInfo>>), 200)]
        public async Task<IActionResult> GetUavModelAsync()
        {
            var data = await _uav.GetAnyAsync();
            var def = new List<UAVModelInfo>() { new UAVModelInfo() { Id = -99, Name = "全部" } };
            if (data?.Count() > 0)
            {
                data = def.Concat(data);
            }
            else
            {
                data = def;
            }
            return Ok(data);
        }

        [HttpPost("uavmodel")]
        [AllowAnonymous]
        public async Task<IActionResult> AddUavModelAsync(string name)
        {
            return Ok(await _uav.AddAsync(new UAVModelAdd() { Name = name }));
        }

        /// <summary>
        /// 根据目标编号获取轨迹数据
        /// </summary>
        /// <param name="tgId">目标编号</param>
        /// <returns></returns>
        [HttpGet("track/{tgId}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<IEnumerable<HistoryTrkInfo>>), 200)]
        public async Task<IActionResult> GetTrackAsync([FromRoute] string tgId)
        {
            return Ok(await _track.GetTracks(tgId));
        }

        /// <summary>
        /// 历史目标统计(上一天)
        /// </summary>
        /// <param name="category">统计类别(0:目标类型；1：持续时长；2：轨迹点数；3：威胁等级；4：目标数)</param>
        /// <returns></returns>
        [HttpGet("summary/{category}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<IEnumerable<HistoryTargetRangSummary>>), 200)]
        public async Task<IActionResult> GetSummaryAsync([FromRoute] SummaryCategory category)
        {
            //时间类型（0:day;1:week;2:month;3:season;4:year）
            var _sumData = await _summary.GetHistorySummary(category, SummaryTimeCategory.Day);
            if (_sumData == null || _sumData.Count() == 0) return default;
            if (category == SummaryCategory.KeepTime || category == SummaryCategory.TrackCount)
            {
                var _avgData = _sumData.Where(f => f.Key.Contains(".avg"));
                var _maxData = _sumData.Where(f => f.Key.Contains(".max"));
                var _minData = _sumData.Where(f => f.Key.Contains(".min"));
                return Ok(_avgData?.Select(f => new HistoryTargetRangSummary()
                {
                    Index = f.Id,
                    Category = category,
                    Title = f.Key.Split(".")[0],
                    Value = f.Value,
                    MaxValue = (double)_maxData?.FirstOrDefault(g => g.Key.Split(".")[0] == f.Key.Split(".")[0])?.Value,
                    MinValue = (double)_minData?.FirstOrDefault(g => g.Key.Split(".")[0] == f.Key.Split(".")[0])?.Value,
                    Date = f.Timestamp
                }));
            }
            return Ok(_sumData.Select(f => new HistoryTargetSummary(f.Id, f.Key, category, f.Value, f.Timestamp)));
        }

        /// <summary>
        ///  历史目标统计 （一周）
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("summary/week/{category}")]
        [Authorize(Roles = SystemRole.Admin)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<IEnumerable<HistoryTargetRangSummary>>), 200)]
        public async Task<IActionResult> GetWeekSummaryAsync([FromRoute] SummaryCategory category)
        {
            //时间类型（0:day;1:week;2:month;3:year）
            var _sumData = await _summary.GetHistorySummary(category, SummaryTimeCategory.Week, false);
            if (_sumData == null || _sumData.Count() == 0) return default;
            if (category == SummaryCategory.KeepTime || category == SummaryCategory.TrackCount)
            {
                var _avgData = _sumData.Where(f => f.Key == "day.avg");
                var _maxData = _sumData.Where(f => f.Key == "day.max");
                var _minData = _sumData.Where(f => f.Key == "day.min");
                return Ok(_avgData?.Select(f => new HistoryTargetRangSummary()
                {
                    Index = f.Id,
                    Category = category,
                    Title = ((int)(DateTime.Parse(f.Timestamp).DayOfWeek)) + "",
                    Value = f.Value,
                    MaxValue = (double)_maxData?.FirstOrDefault(g => g.Key.Split(".")[0] == f.Key.Split(".")[0])?.Value,
                    MinValue = (double)_minData?.FirstOrDefault(g => g.Key.Split(".")[0] == f.Key.Split(".")[0])?.Value,
                    Date = f.Timestamp
                }));
            }
            if (category == SummaryCategory.TargetCount)
            {
                return Ok(_sumData?.Select(f => new HistoryTargetSummary()
                {
                    Index = f.Id,
                    Category = category,
                    Title = ((int)(DateTime.Parse(f.Timestamp).DayOfWeek)) + "",
                    Value = f.Value,
                    Date = f.Timestamp
                }));
            }
            var _groups = _sumData.GroupBy(f => f.Key);
            return Ok(_groups.Select(f => new HistoryTargetSummary()
            {
                Category = category,
                Title = f.Key + "",
                Value = _sumData.Where(g => g.Key == f.Key).Sum(gg => gg.Value)
            }));
        }

        /// <summary>
        ///  历史目标统计 （一月）
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("summary/month/{category}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<IEnumerable<HistoryTargetRangSummary>>), 200)]
        public async Task<IActionResult> GetMonthSummaryAsync([FromRoute] SummaryCategory category)
        {
            //时间类型（0:day;1:week;2:month;3:season;4:year）
            var _sumData = await _summary.GetHistorySummary(category, SummaryTimeCategory.Month, false);
            if (_sumData == null || _sumData.Count() == 0) return default;
            if (category == SummaryCategory.KeepTime || category == SummaryCategory.TrackCount)
            {
                var _avgData = _sumData.Where(f => f.Key.Contains(".avg"));
                var _maxData = _sumData.Where(f => f.Key.Contains(".max"));
                var _minData = _sumData.Where(f => f.Key.Contains(".min"));
                return Ok(_avgData?.Select(f => new HistoryTargetRangSummary()
                {
                    Index = f.Id,
                    Category = category,
                    Title = DateTime.Parse(f.Timestamp).Day + "",
                    Value = f.Value,
                    MaxValue = (double)_maxData?.FirstOrDefault(g => g.Timestamp == f.Timestamp)?.Value,
                    MinValue = (double)_minData?.FirstOrDefault(g => g.Timestamp == f.Timestamp)?.Value,
                    Date = f.Timestamp
                }));
            }
            if (category == SummaryCategory.TargetCount)
            {
                return Ok(_sumData?.Select(f => new HistoryTargetSummary()
                {
                    Index = f.Id,
                    Category = category,
                    Title = DateTime.Parse(f.Timestamp).Day + "",
                    Value = f.Value,
                    Date = f.Timestamp
                }));
            }
            var _groups = _sumData.GroupBy(f => f.Key);
            return Ok(_groups.Select(f => new HistoryTargetSummary()
            {
                Category = category,
                Title = f.Key + "",
                Value = _sumData.Where(g => g.Key == f.Key).Sum(gg => gg.Value),
                Date = DateTime.Parse(_sumData.FirstOrDefault().Timestamp).ToString("yyyy-MM")
            }));
        }

        /// <summary>
        ///  历史目标统计 （一年）
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("summary/year/{category}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(ServiceResponse<IEnumerable<HistoryTargetRangSummary>>), 200)]
        public async Task<IActionResult> GetYearSummaryAsync([FromRoute] SummaryCategory category)
        {
            //时间类型（0:day;1:week;2:month;3:season;4:year）
            var _sumData = await _summary.GetHistorySummary(category, SummaryTimeCategory.Year, false);
            if (_sumData == null || _sumData.Count() == 0) return default;
            var _year = DateTime.Parse(_sumData.FirstOrDefault().Timestamp).Year;
            var _months = _sumData.GroupBy(f => DateTime.Parse(f.Timestamp).Month);
            if (category == SummaryCategory.KeepTime || category == SummaryCategory.TrackCount)
            {
                return Ok(_months?.Select(_month =>
                {
                    var _avgData = _sumData.Where(f => f.Key.Contains(".avg") && DateTime.Parse(f.Timestamp).Month == _month.Key);
                    var _maxData = _sumData.Where(f => f.Key.Contains(".max") && DateTime.Parse(f.Timestamp).Month == _month.Key);
                    var _minData = _sumData.Where(f => f.Key.Contains(".min") && DateTime.Parse(f.Timestamp).Month == _month.Key);
                    return new HistoryTargetRangSummary()
                    {
                        Index = _month.Key,
                        Category = category,
                        Title = _month.Key + "",
                        Value = _avgData.Average(f => f.Value),
                        MaxValue = _maxData.Max(f => f.Value),
                        MinValue = _minData.Min(f => f.Value),
                        Date = $"{_year}-{_month.Key}"
                    };
                }));

            }
            if (category == SummaryCategory.TargetCount)
            {
                return Ok(
                    _months?.Select(_month =>
                    {
                        var _monthData = _sumData.Where(f => DateTime.Parse(f.Timestamp).Month == _month.Key);
                        return new HistoryTargetSummary(_month.Key, _month.Key + "", category, _monthData.Sum(f => f.Value), $"{_year}-{_month.Key}");
                    }));
            }
            var _groups = _sumData.GroupBy(f => f.Key);
            return Ok(_groups.Select(f => new HistoryTargetSummary()
            {
                Category = category,
                Title = f.Key + "",
                Value = _sumData.Where(g => g.Key == f.Key).Sum(gg => gg.Value),
                Date = _year + ""
            }));
        }

        /// <summary>
        /// 清理历史目标数据
        /// </summary>
        /// <param name="id">目标编号</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public IActionResult Clean([FromRoute] int id)
        {
            //清理历史目标数据
            //备份数据（获取上次备份时间）
            //清理数据
            try
            {
                _history.DelAsync(id);
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}清除历史数据{id}。");
                return Ok(true);

            }
            catch (Exception ex)
            {

                return BadRequest(new BussinessException(BussinessExceptionCode.OptDelFail));

            }
        }

        /// <summary>
        /// 清理全部历史目标数据
        /// </summary>
        /// <param name="id">目标编号</param>
        /// <returns></returns>
        [HttpDelete("clear")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<bool>), 200)]
        public IActionResult CleanAll([FromBody] IEnumerable<int> ids)
        {
            try
            {
                _history.DelAsync(ids);
                _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}批量清除历史数据。");
                return Ok(true);
            }
            catch (Exception ex)
            {

                return BadRequest(new BussinessException(BussinessExceptionCode.OptDelFail));

            }
        }

        /// <summary>
        /// 导出历史数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>生成文件访问路径</returns>
        [HttpPost("Export")]
        //[Authorize(Roles = SystemRole.Admin)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> ExportHistoryTarget([FromBody] IEnumerable<int> ids)
        {
            if (ids?.Count() <= 0)
            {
                return BadRequest(new BussinessException(BussinessExceptionCode.ParamInvalidId));
            }
            var his = await _history.GetAnyAsync<HistoryTgInfo>(x => ids.Contains(x.Id));
            var sWebRootFolder = $"{AppContext.BaseDirectory}export/history/";
            if (!Directory.Exists(sWebRootFolder))
                Directory.CreateDirectory(sWebRootFolder);
            string sFileName = $"{Guid.NewGuid()}.xlsx";
            string template = Path.Combine(sWebRootFolder, "template.xlsx");
            var newFile = Path.Combine(sWebRootFolder, sFileName);
            var models = await _uav.GetAnyAsync();
            if (!System.IO.File.Exists(template))
            {
                return BadRequest(new BussinessException(BussinessExceptionCode.TemplateNotFound));
            }
            System.IO.File.Copy(template, newFile, true);
            if (!System.IO.File.Exists(newFile))
            {
                return BadRequest(new BussinessException(BussinessExceptionCode.CreateFileFail));
            }
            FileInfo file = new FileInfo(newFile);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                int index = 1;
                foreach (var tg in his)
                {
                    worksheet.Cells[index + 3, 2].Value = index;//序号
                    worksheet.Cells[index + 3, 3].Value = tg.Starttime.ToString("yyyy/MM/dd");//日期
                    worksheet.Cells[index + 3, 4].Value = tg.Sn;//sn
                    worksheet.Cells[index + 3, 5].Value = models.FirstOrDefault(x => x.Id == tg.UAVModel)?.Name ?? "未知(0)";//类型

                    var hit = Convert.ToBoolean(tg.HitMark);//反制情况
                    worksheet.Cells[index + 3, 7].Value = hit ? "已反制" : "未操作";

                    var threat = GetThreatInfo(tg.ThreatMax);//告警等级
                    //var s = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    if (threat == 1)
                    {
                        worksheet.Cells[index + 3, 6].Value = "红色告警";
                        //worksheet.Cells[index + 3, 6].Style.Fill.PatternType = s;
                        //worksheet.Cells[index + 3, 6].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        //worksheet.Cells[index + 3, 7].Style.Fill.PatternType = s;
                        //worksheet.Cells[index + 3, 7].Style.Fill.BackgroundColor.SetColor(hit ? Color.LightGreen : Color.Red);
                    }
                    else if (threat == 2)
                    {
                        worksheet.Cells[index + 3, 6].Value = "蓝色告警";
                        //worksheet.Cells[index + 3, 6].Style.Fill.PatternType = s;
                        //worksheet.Cells[index + 3, 6].Style.Fill.BackgroundColor.SetColor(Color.DodgerBlue);
                        //worksheet.Cells[index + 3, 7].Style.Fill.PatternType = s;
                        //worksheet.Cells[index + 3, 7].Style.Fill.BackgroundColor.SetColor(hit ? Color.LightGreen : Color.Red);
                    }
                    else
                    {
                        worksheet.Cells[index + 3, 6].Value = "无告警";
                    }

                    worksheet.Cells[index + 3, 8].Value = tg.Starttime.ToString("HH:mm:ss");//起
                    worksheet.Cells[index + 3, 9].Value = tg.Endtime.ToString("HH:mm:ss");//止
                    worksheet.Cells[index + 3, 10].Value = tg.FlyerPosition;//飞手位置
                    index++;
                }

                package.Save();
            }
            _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}下载历史目标数据报表。");
            return Ok($"/export/history/{sFileName}");
            //var stream = System.IO.File.OpenRead(newFile);
            //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"反制历史数据{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        }

        private int GetThreatInfo(double threat)
        {
            if (threat > 60)
                return 1;
            else if (threat > 30)
                return 2;
            else
                return 3;
        }

        /// <summary>
        /// 导出操作日志
        /// </summary>
        /// <param name="datestr">日志时间</param>
        /// <returns>日志下载路径</returns>
        [HttpGet("ExportLog/{datestr}")]
        [Authorize(Roles = SystemRole.Admin)]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult ExportLog([FromRoute] string datestr)
        {
            if (!DateTime.TryParse(datestr, out DateTime date))
            {
                return BadRequest(new BussinessException(BussinessExceptionCode.ParamInvalidId));
            }
            if (date == null || date.Year < 2021)
            {
                return BadRequest(new BussinessException(BussinessExceptionCode.ParamInvalidId));
            }
            var sWebRootFolder = $"{AppContext.BaseDirectory}export/optlog/";
            if (!Directory.Exists(sWebRootFolder))
                Directory.CreateDirectory(sWebRootFolder);

            var logfile = $"{AppContext.BaseDirectory}logs/nlog-opt-{date.ToString("yyyy-MM-dd")}.log";
            if (!System.IO.File.Exists(logfile))
            {
                return BadRequest(new BussinessException(BussinessExceptionCode.LogFileNotFound));
            }
            var sFileName = $"{Guid.NewGuid()}.xlsx";
            var newFile = Path.Combine(sWebRootFolder, sFileName);
            using (var sr = new StreamReader(logfile))
            {
                var line = sr.ReadLine();
                FileInfo file = new FileInfo(newFile);
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    // 添加worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"{date.ToString("yyyy-MM-dd")}操作日志");

                    int index = 1;
                    while (!string.IsNullOrWhiteSpace(line))
                    {
                        var ldata = line.Split("|");
                        worksheet.Cells[index, 1].Value = index;//序号
                        worksheet.Cells[index, 2].Value = ldata[0];//日期
                        worksheet.Cells[index, 3].Value = ldata[2];//sn
                        worksheet.Cells[index, 4].Value = ldata[3];//类型
                        worksheet.Cells[index, 5].Value = ldata[4];//起
                        worksheet.Cells[index, 6].Value = ldata[5];//止
                        index++;
                        line = sr.ReadLine();
                    }

                    package.Save();
                }
            }

            _logger.LogInformation($"用户{HttpContext.GetCurrentUsername()}下载{date.ToString("yyyy-MM-dd")}的操作日志。");
            return Ok($"/export/optlog/{sFileName}");
        }

        /// <summary>
        /// 导出全部历史目标数据
        /// </summary>
        /// <param name="id">目标编号</param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("Export")]
        //[AllowAnonymous]
        //[ProducesResponseType(typeof(FileResult), 200)]
        //public async Task<IActionResult> ExportAll()
        //{
        //    var his=await _history.GetAnyAsync<HistoryTgInfo>();
        //    //var fs= SaveTxt(his.ToList());
        //    var res = await GetAllResult(his.ToList());


        //    var actionresult = new FileStreamResult(res, new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("text/plain"));
        //    actionresult.FileDownloadName = $"TargetHis_{DateTime.Now.ToFileTime()}.txt";


        //    return Ok(actionresult);
        //}
        //private async Task<Stream> GetAllResult(List<HistoryTgInfo> historyTgInfos)
        //{
        //    var stream = new MemoryStream();
        //    var writer = new StreamWriter(stream);
        //    //生成内容
        //    for (int i = 0; i < historyTgInfos.Count; i++)
        //    {
        //        writer.WriteLine(historyTgInfos[i].ToJson());
        //    }
        //    writer.Flush();
        //    stream.Position = 0;
        //    return stream;
        //}
        //private string SaveTxt(List<HistoryTgInfo> historyTgInfos)
        //{
        //    string rootPath = Path.GetDirectoryName((new Program()).GetType().Assembly.Location);
        //    string path = $"{rootPath}\\HistoryTargetRecord\\{DateTime.Now.ToFileTime()}_his.txt";
        //    //创建一个文件流，用以写入或者创建一个StreamWriter 
        //    FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        //    StreamWriter sw = new StreamWriter(fs);
        //    sw.Flush();
        //    // 使用StreamWriter来往文件中写入内容 
        //    sw.BaseStream.Seek(0, SeekOrigin.Begin);
        //    for (int i = 0; i < historyTgInfos.Count; i++) sw.WriteLine(historyTgInfos[i].ToJson());
        //    //关闭此文件t 
        //    sw.Flush();
        //    sw.Close();
        //    fs.Position = 0;
        //    fs.Close();
        //    return path;
        //}

    }

    public class HeatMapResponse
    {
        public TrackList[] trackList { get; set; }
    }

    public class TrackList
    {
        public Point point { get; set; }
        public string address { get; set; }
    }

    public class Point
    {
        public string lng { get; set; }
        public string lat { get; set; }
    }


    
    public class Weigui
    {
        public string name { get; set; }
        public int value { get; set; }
        public int score { get; set; }
    }


}

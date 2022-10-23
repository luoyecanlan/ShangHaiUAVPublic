using LinqToDB.Mapping;
using DbOrm.CRUD;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
	[Table("summary")]
	public abstract class SummaryBase { }

	public abstract class SummaryKeyBase : SummaryBase, IEntityKeyProperty
	{
		[Column("id"),PrimaryKey,Identity] public int Id { get; set; } // int(11)
	}

	public partial class SummaryKey : SummaryKeyBase { }
	public partial class SummaryAdd:SummaryBase
	{
		/// <summary>
		/// 0：类型；1：持续时长；2：轨迹点数；3：威胁等级；4：目标数
		/// </summary>
		[Column("category"), NotNull] public int Category { get; set; } // int(11)
		/// <summary>
		/// 统计指标项键
		/// </summary>
		[Column("key"), NotNull] public string Key { get; set; } // varchar(255)
		/// <summary>
		/// 统计结果
		/// </summary>
		[Column("value"), NotNull] public double Value { get; set; } // int(11)
		/// <summary>
		/// 时间标签;格式（yyyyMMdd）
		/// </summary>
		[Column("timestamp"), NotNull] public string Timestamp { get; set; } // varchar(8)
		/// <summary>
		/// 创建时间
		/// </summary>
		[Column("createtime"), NotNull] public DateTime Createtime { get; set; } // datetime
	}
	public partial class SummaryInfo : SummaryKeyBase
	{
		/// <summary>
		/// 0：类型；1：持续时长；2：轨迹点数；3：威胁等级；4：探测设备数
		/// </summary>
		[Column("category"), NotNull] public int Category { get; set; } // int(11)
		/// <summary>
		/// 统计指标项键
		/// </summary>
		[Column("key"), NotNull] public string Key { get; set; } // varchar(255)
		/// <summary>
		/// 统计结果
		/// </summary>
		[Column("value"), NotNull] public double Value { get; set; } // int(11)
		/// <summary>
		/// 时间标签;格式（yyyyMMdd）
		/// </summary>
		[Column("timestamp"), NotNull] public string Timestamp { get; set; } // varchar(8)
		/// <summary>
		/// 创建时间
		/// </summary>
		[Column("createtime"), NotNull] public DateTime Createtime { get; set; } // datetime
	}
}

using DbOrm.CRUD;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrm.AntiUAV.Entity
{
    [Table("user_cms_personalization")]
    public abstract class PersonalizationBase { }

    public abstract class PersonalizationKeyBase : PersonalizationBase, IEntityKeyProperty
    {
        [Column("id"), PrimaryKey, Identity]
        public int Id { get; set; }
    }
    public class PersonalizationAdd : PersonalizationBase
    {
        [Column("user_id"), NotNull] public int UserId { get; set; } // int(11)
        [Column("map_id"), NotNull] public int MapId { get; set; } // int(11)
        [Column("device_cover"), NotNull] public string DeviceCover { get; set; } // varchar(1000)
        [Column("device_line"), NotNull] public string DeviceLine { get; set; } // varchar(1000)
        [Column("filter_v_min"), NotNull] public int FilterVMin { get; set; } // int(11)
        [Column("filter_v_max"), NotNull] public int FilterVMax { get; set; } // int(11)
        [Column("filter_disc_min"), NotNull] public int FilterDiscMin { get; set; } // int(11)
        [Column("filter_disc_max"), NotNull] public int FilterDiscMax { get; set; } // int(11)
        [Column("filter_alt_min"), NotNull] public int FilterAltMin { get; set; } // int(11)
        [Column("filter_alt_max"), NotNull] public int FilterAltMax { get; set; } // int(11)
        [Column("filter_threat"), NotNull] public string FilterThreat { get; set; } // varchar(1000)
        [Column("filter_category"), NotNull] public string FilterCategory { get; set; } // varchar(1000)
        [Column("other"), NotNull] public string Other { get; set; } // varchar(2000)
        [Column("updatetime"), NotNull] public DateTime Updatetime { get; set; } // datetime
    }

    public class PersonalizationUpdate : PersonalizationKeyBase
    {
        [Column("map_id"), NotNull] public int MapId { get; set; } // int(11)
        [Column("device_cover"), NotNull] public string DeviceCover { get; set; } // varchar(1000)
        [Column("device_line"), NotNull] public string DeviceLine { get; set; } // varchar(1000)
        [Column("filter_v_min"), NotNull] public int FilterVMin { get; set; } // int(11)
        [Column("filter_v_max"), NotNull] public int FilterVMax { get; set; } // int(11)
        [Column("filter_disc_min"), NotNull] public int FilterDiscMin { get; set; } // int(11)
        [Column("filter_disc_max"), NotNull] public int FilterDiscMax { get; set; } // int(11)
        [Column("filter_alt_min"), NotNull] public int FilterAltMin { get; set; } // int(11)
        [Column("filter_alt_max"), NotNull] public int FilterAltMax { get; set; } // int(11)
        [Column("filter_threat"), NotNull] public string FilterThreat { get; set; } // varchar(1000)
        [Column("filter_category"), NotNull] public string FilterCategory { get; set; } // varchar(1000)
        [Column("updatetime"), NotNull] public DateTime Updatetime { get; set; } // datetime
    }

    public class PersonalizationDel : PersonalizationKeyBase
    {

    }

    public class PersonalizationInfo : PersonalizationKeyBase
    {
        [Column("user_id"), NotNull] public int UserId { get; set; } // int(11)
        [Column("map_id"), NotNull] public int MapId { get; set; } // int(11)
        [Column("device_cover"), NotNull] public string DeviceCover { get; set; } // varchar(1000)
        [Column("device_line"), NotNull] public string DeviceLine { get; set; } // varchar(1000)
        [Column("filter_v_min"), NotNull] public int FilterVMin { get; set; } // int(11)
        [Column("filter_v_max"), NotNull] public int FilterVMax { get; set; } // int(11)
        [Column("filter_disc_min"), NotNull] public int FilterDiscMin { get; set; } // int(11)
        [Column("filter_disc_max"), NotNull] public int FilterDiscMax { get; set; } // int(11)
        [Column("filter_alt_min"), NotNull] public int FilterAltMin { get; set; } // int(11)
        [Column("filter_alt_max"), NotNull] public int FilterAltMax { get; set; } // int(11)
        [Column("filter_threat"), NotNull] public string FilterThreat { get; set; } // varchar(1000)
        [Column("filter_category"), NotNull] public string FilterCategory { get; set; } // varchar(1000)
        [Column("other"), NotNull] public string Other { get; set; } // varchar(2000)
        [Column("updatetime"), NotNull] public DateTime Updatetime { get; set; } // datetime
    }
}

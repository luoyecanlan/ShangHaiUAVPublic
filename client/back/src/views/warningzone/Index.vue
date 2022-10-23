<template>
  <div id="zone" class="zoneBox">
    <div class="zone-list">
      <div class="table-container">
        <zone-list ref="zoneList" @onDataChanged="onZoneListChange" />
      </div>
    </div>
    <div class="map-box">
      <div class="option-box">
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <map-option
          @onMoChanged="onZoneListChange"
          @onNameChanged="onlyInitData"
          class="mapOption"
          ref="mapOption"
          @drawTypeChange="drawTypeChanged"
        />
      </div>
      <div id="map"></div>
    </div>
  </div>
</template>

<script>
import "ol/ol.css";
import {
  Init_Map,
  Get_Layer,
  Register_Modify_Event,
  Register_Draw_Event,
  Unregister_Draw_Event,
  Draw_Zone,
} from "./js";

import {
  http_request,
  update_warn_zone_area,
  request_all_warn_zones,
} from "../../modes/api";
import { toLonLat } from "ol/proj";
import List from "../warningzone/ZoneList";
import OptionPanel from "./MapOption";
import { show_message, msg_enum } from "../../modes/elementUI";
import Bus from "../../modes/tool/bus";
import { event_type } from "../../modes/tool/bus";
import { zoneInitType } from "../../modes/tool";

export default {
  name: "Index",
  data() {
    return {
      map: {},
      zones: [],
      zoneLayer: undefined,
      drawLayer: undefined,
      // options: [
      //   { id: 3, label: "多边形", value: "Polygon" },
      //   { id: 4, label: "圆形", value: "Circle" },
      //   { id: 5, label: "取消", value: "None" }
      // ],
      drawType: "None",
      zoneInfo: undefined,
    };
  },
  components: {
    MapOption: OptionPanel,
    ZoneList: List,
  },
  methods: {
    onlyInitData(data) {
      this.$refs.zoneList._initData(data);
    },
    refreshData(data) {
      console.log("*999*", data);
      const { type, id } = data;
      let _info;
      console.log(data);
      if (type === zoneInitType.updated || type === zoneInitType.created) {
        _info = this.zones.find((f) => f.id === id);
      } else {
        _info = this.zones[0];
      }
      this.initDrawDataAndLayer(_info.id);
    },
    onZoneListChange(data) {
      console.log("onZoneListChange", data);
      this.initZoneDataAndView(() => {
        this.refreshData(data);
      });
    },
    //生成Id
    createFeatureId(info) {
      const { id, name } = info;
      return `${name}_${id}`;
    },
    //保存数据
    saveZoneData(__feature_, _zoneInfo_) {
      let _id__ = this.createFeatureId(_zoneInfo_);
      //整体数据
      if (!__feature_) return;
      let _data = this.filterCoreData(__feature_);
      __feature_.setId(_id__);
      let ZonePointsPosition= this.formatePosition(_data) 
      let __zoneInfo = {
        id: _zoneInfo_.id,
        zonePoints:JSON.stringify(_data),
        ZonePointsPosition:JSON.stringify(ZonePointsPosition),
      };
      //提交数据库
        http_request(update_warn_zone_area, __zoneInfo, () => {
          show_message(msg_enum.success, this.$t('m.zoneModifySuccess'));
          //修改完成后刷新数据
          let data= {
            type: zoneInitType.updated,
              id:_zoneInfo_.id
          };
          this.initZoneDataAndView(()=>{this.refreshData(data)});
          //Bus.$emit(event_type.init_zone_data,)
      });
    },

    /**
     * 格式画多边形坐标数据  xy->lat,lng
     */
    formatePosition(_info) {
      const { type, coordinates, layout } = _info;
      if (coordinates && coordinates.length) {
        let firstCoordinate = coordinates[0];
        let toCoordinate=[]
        if (firstCoordinate.length) {
           toCoordinate=firstCoordinate.map((f) => {
            const[Lng,Lat]= toLonLat(f);
            return {Lng,Lat}
          });
        }
        return toCoordinate
      }
    },

    //过滤返回关键数据
    filterCoreData(_feature) {
      let _drawType = _feature.drawType;
      let _geom = _feature.getGeometry();
      let _data = {
        type: _drawType,
      };
      //判断当前绘制图形类型  保存关键信息
      if (_drawType === "Circle") {
        //Circle  （center,radius,layout）
        _data = {
          ..._data,
          center: _geom.getCenter(),
          radius: _geom.getRadius(),
          layout: _geom.getLayout(),
        };
      } else if (_drawType === "Polygon") {
        //Polygon （coordinates，ends，layout）
        _data = {
          ..._data,
          coordinates: _geom.getCoordinates(true),
          layout: _geom.getLayout(),
        };
      } else {
      }
      return _data;
    },
    //绘制类型改变
    drawTypeChanged(type) {
      this.drawType = type;
      Unregister_Draw_Event();
      if (type !== "None") {
        Register_Draw_Event(type, (e) => {
          //绘制结束
          let _feature = e.feature;
          _feature.drawType = type;
          this.drawType = "None";
          this.saveZoneData(_feature, this.zoneInfo);
        });
      }
    },
    //初始化告警区域数据
    initZoneDataAndView(callback) {
      // console.log("initDrawDataAndLayer",_data_)
      // let type,id;
      this.zoneLayer.getSource().clear(true);
      //获取数据库数据 初始化地图数据
      http_request(request_all_warn_zones, null, (_data) => {
        this.zones = _data;
        // const {id,type}=_data_;
        // let _info=undefined;
        // callback||callback();
        if (this.zones && this.zones.length) {
          // this.zoneLayer.getSource().clear();
          this.zones.forEach((__info_) => {
            Draw_Zone(this.zoneLayer, __info_);
          });
        }
        //   if(id){
        //     _info=this.zones.find(f=>f.id===id);
        //   }
        //   if(!_info){
        //     _info=this.zones[0];
        //   }
        // }
        //修改编辑图形
        // this.initDrawDataAndLayer(_info,type);
        callback && callback();
      });
    },
    //绘制图形并显示图层
    initDrawDataAndLayer(id) {
      //绘制层添加图形
      let _info = this.zones.find((f) => f.id === id);

      console.log("select 01", _info, this.zones);
      this.drawLayer.getSource().clear();
      if (_info) {
        this.initOptionPanelData(_info);
        if (_info.zonePoints) {
          Draw_Zone(this.drawLayer, _info, true);
        }
      }
    },
    //初始化操作面板基础数据
    initOptionPanelData(_info) {
      this.zoneInfo = _info;
      if (this.$refs.mapOption) {
        this.$refs.mapOption.initData(_info);
      }
    },
  },
  mounted() {
    //初始化地图
    Init_Map("map");
    this.zoneLayer = Get_Layer(false);
    this.drawLayer = Get_Layer(true);
    //修改绘制相关
    Register_Modify_Event((e) => {
      let _feature = e.features.item(0);
      let _id_ = _feature.getId();
      let tempArr = _id_.split("_");
      this.saveZoneData(_feature, {
        name: tempArr[0],
        id: parseInt(tempArr[1]),
      });
    });
    Bus.$off(event_type.begin_draw_zone);
    //开启图形编辑
    Bus.$on(event_type.begin_draw_zone, (id) => {
      this.initDrawDataAndLayer(id);
    });

    // Bus.$off(event_type.reload_zone_data);
    // Bus.$on(event_type.reload_zone_data, data => {
    //   console.log("*8*",data);
    //   this.initZoneDataAndView(()=>{
    //     console.log("*999*",data);
    //     const {type,id}=data;
    //     let _info;
    //     console.log(data);
    //     if(type===zoneInitType.updated||type===zoneInitType.created){
    //       _info= this.zones.find(f=>f.id===id);
    //     }else{
    //       _info= this.zones[0];
    //     }
    //     this.initDrawDataAndLayer(_info);
    //   });
    // });
    // this.initZoneDataAndView(()=>{
    //   this.initDrawDataAndLayer(this.zones[0]);
    // });
  },
};
</script>

<style scoped>
.zoneBox {
  position: relative;
  height: 100%;
  width: 100%;
  display: flex;
  flex-direction: row;
}
.zone-list {
  width: 485px;
  position: relative;
  display: flex;
  flex-direction: column;
}
.table-container {
  flex: 1;
}
.map-box {
  flex: 1;
  display: flex;
  flex-direction: column;
  margin-left: 20px;
  margin-top: 8px;
  z-index: 100;
}
.mapOption {
  position: absolute;
}
/*.option-box {*/
/*  height: 41px;*/
/*  !*background-image: url("../../assets/main/map-header-bg.png");*!*/
/*  !*background-size: ;*!*/
/*  background-repeat: no-repeat;*/
/*}*/
.option-box {
  height: 41px;
  display: flex;
  flex-direction: row;
  position: relative;
}
.option-box div:nth-child(1) {
  background-image: url("../../assets/main/option-left.png");
  width: 31px;
  height: 41px;
}
.option-box div:nth-child(2) {
  background-image: url("../../assets/main/option-main-bg.png");
  flex: 1;
  height: 41px;
}
.option-box div:nth-child(3) {
  background-image: url("../../assets/main/option-right.png");
  width: 50px;
  height: 41px;
}
.option-box div:nth-child(4) {
  background-image: url("../../assets/main/option-bg.png");
  width: 500px;
  height: 41px;
}

/*.option-box:before{*/
/*  content: "选择需要修改的告警区域";*/
/*  color: #40cef9;*/
/*  line-height: 70px;*/
/*  padding-left: 40px;*/
/*  font-size: 24px;*/
/*  opacity: .2;*/
/*  position: absolute;*/
/*}*/
/*.option-box {*/
/*  position: fixed;*/
/*  z-index: 99;*/
/*  top: 0px;*/
/*  left: 0px;*/
/*  !*height: 70px;*!*/
/*  !*background-image: url("../../assets/main/map-header-bg.png");*!*/
/*  !*background-repeat: no-repeat;*!*/
/*}*/
#map {
  flex: 1;
  border: 5px solid #40cef9;
}
.ol-control button {
  display: none;
}
</style>

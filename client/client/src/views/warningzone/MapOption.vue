<template>
  <transition name="fade">
    <div class="optionBox" v-if="drawInfo">
<!--      区域名称-->
      <el-popover v-model="areaNameVisible"
        placement="bottom"
        trigger="click">
        <div style="height: 32px;">
          <input class="zone-input" v-model="areaName" :placeholder="$t('m.zoneName')"/>
          <button class="zone-button" @click="saveZoneName">{{$t('m.confirm')}}</button>
        </div>
        <span slot="reference" style="width: 120px" class="option-item ellipsis">{{$t('m.name')}}：{{drawInfo.name}}</span>
      </el-popover>
<!--区域形状-->
      <el-popover v-model="area_visible"
        placement="bottom"
        trigger="click">
        <div>
          <p class="zone-area-item" @click="areaChanged(item)" v-for="(item,key) in options" :key="key">
            {{item.label}}
          </p>
        </div>
        <span slot="reference" class="option-item" style="width: 140px">{{$t('m.zoneShape')}}：{{getAreaName()}}</span>
      </el-popover>
<!--      填充颜色-->
      <span class="option-item" style=" width: 100px;">
            {{$t('m.fillColor')}}：
                  <el-color-picker size="medium" style="top: 4px"
                                   v-model="drawInfo.fullColor"
                                   show-alpha @change="onChange">
                  </el-color-picker></span>
<!--      线条颜色-->
      <span class="option-item" style=" width: 130px;" >
       {{$t('m.borderColor')}}：
                  <el-color-picker size="medium" style="top: 4px"
                    v-model="drawInfo.lineColor"
                    show-alpha @change="onChange">
                  </el-color-picker>
      </span>
<!--      线条宽度-->
      <el-popover v-model="line_visible"
        placement="bottom"
        trigger="click">
        <div style="height: 32px;">
          <el-input-number size="small" v-model="drawInfo.lineThickness"
                           :step="0.5" :min="0.5" :max="5">
          </el-input-number>
          <button type="text" size="small" class="zone-button" style="height: 30px;border-left: none" @click="onChange">{{$t('m.confirm')}}</button>
        </div>
        <span slot="reference" class="option-item" style=" width: 140px;" >{{$t('m.borderWidth')}}：{{drawInfo.lineThickness}}</span>
      </el-popover>
    </div>
  </transition>
</template>

<script>
  import Bus from '../../modes/tool/bus'
  import {event_type} from "../../modes/tool/bus";
  import {http_request,update_warn_zone_info} from "../../modes/api";
  import {zoneInitType} from "../../modes/tool";

  export default {
      name: "MapOption",
      data() {
        return {
          options: [
            {id: 3,isDisable:false, label: this.$t('m.polygon'), value: "Polygon"},
            {id: 4,isDisable:false, label: this.$t('m.circular'), value: "Circle"},
            {id: 5,isDisable:false, label: this.$t('m.edit'), value: "None"}
          ],
          areaName:"",
          areaNameVisible:false,
          line_visible:false,
          drawType: "None",
          drawInfo:undefined,
          fullStyle: {
            display: "inline-block",
            height: '16px',
            width: '16px',
            borderRadius: '8px',
            backgroundColor: '#f00'
          },
          lineStyle: {
            display: "inline-block",
            height: '16px',
            width: '16px',
            borderRadius: '8px',
            border: '2px solid #f00'
          },
          area_visible:false
        }
      },
      methods:{
        getAreaName() {
          let __R = this.options.filter(f => f.value === this.drawType);
          if (__R) {
            return __R[0].label || "";
          }
          return "";
        },
        saveZoneName(){
          if(this.areaName){
            this.drawInfo.name=this.areaName;
            this.areaName="";
            this.onChange();
            this.areaNameVisible=false;
          }
          return false;
        },
        areaChanged(e){
          const {value}=e;
          this.drawType=value;
          this.area_visible=false;
          this.$emit("drawTypeChange",this.drawType);
          return false;
        },
        initData(_info){
          if(_info){
            this.drawInfo=_info;
            //如果存在图形 则进行修改
            if(_info.zonePoints){
              this.drawType='None';
              this.options.map(f=>{
                if(f.value!=='None')f.isDisable=true;
              });
            }else{
              //如果不存在图形  默认选中多边形进行绘制
              this.drawType='Polygon';
              this.options.map(f=>{
                f.isDisable=false;
              });
            }
            this.$emit("drawTypeChange",this.drawType);
          }
        },
        onChange(){
          if(!this.drawInfo)return false;
          this.line_visible=false;
          //保存数据
          const{id,name,lineColor,lineThickness,fullColor}=this.drawInfo;
          http_request(update_warn_zone_info,{id,name,lineColor,lineThickness,fullColor},__data=> {
            //debugger
            this.$emit('onNameChanged', {type: zoneInitType.updated, id: __data.id});
            this.$emit('onMoChanged', {type: zoneInitType.updated, id: __data.id});
            // Bus.$emit(event_type.reload_zone_data, {type: zoneInitType.updated, id: __data.id});
          });
        }
      },
      mounted() {
      }
    }
</script>

<style scoped>
  .optionBox{
    width: 780px;
    padding-left: 33px;
  }
  .option-item{
    line-height: 30px;
    margin-top: 5px;
    width: 110px;
    margin-right: 10px;
    color: #40cef9;
    font-size: 14px;
    display: block;
    float: left;
    cursor: pointer;
  }
  .none{
    font-size: 24px;
    color: #40cef9;
  }
  .zone-input{
    background-color: transparent;
    height: 32px;
    line-height: 1;
    padding-left: 10px;
    width: 120px;
    border: none;
    color: #40cef9;
    font-size: 14px;
  }
  .zone-input:focus{
    outline: 0;
  }
  .zone-button {
    position: relative;
    top: -2px;
    width: 50px;
    height: 32px;
    color: #40cef9;
    border: none;
    border-left: .5px solid rgba(64,204,249,0.4);
    background-color: transparent;
    font-size: 14px;
    cursor: pointer;
  }
  .zone-button:hover{
    background-color: rgba(2,14,44,0.4);
  }

  .zone-area-item{
    line-height: 30px;
    font-size: 14px;
    padding-left: 10px;
    cursor: pointer;
  }
  .zone-area-item:hover{
    background-color: rgba(64,204,249,0.3);
  }
  /*.item-row{*/
  /*  margin-top: 10px;*/
  /*}*/
  /*.item-title{*/
  /*  padding-left: 5px;*/
  /*  line-height: 44px;*/
  /*  color: #1DA57A;*/
  /*  font-size: 14px;*/
  /*}*/
  /*.item-content{*/
  /*  padding-left: 5px;*/
  /*  line-height: 44px;*/
  /*  font-size: 14px;*/
  /*}*/
</style>

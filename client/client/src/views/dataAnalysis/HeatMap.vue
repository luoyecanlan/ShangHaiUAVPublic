<template>
  <div>
    <div class="center">一周内无人机热力图</div>
    <baidu-map class="heatmap"
               :center="center"
               :zoom="zoom"
               :mapStyle="mapStyle"
               :scroll-wheel-zoom="true"
               @ready="handler">
      <bml-heatmap :data="data" :max="100" :radius="20">
      </bml-heatmap>
      <!-- 缩放控件，注册此组件才会显示拖放进度 -->
      <bm-navigation anchor="BMAP_ANCHOR_TOP_RIGHT"></bm-navigation>
<!--      <div v-for="(marker, index) in markers" :key="marker.id">-->
    </baidu-map>
  </div>

</template>

<script>
import { BmlHeatmap } from 'vue-baidu-map'
import {http_request, request_his_heatMapData, request_his_targets} from "../../modes/api";
export default {
  name: "HeatMap",
  components: {
    BmlHeatmap
  },
  data() {
    return {
      makers:[{
        lng:121,lat:31,showFlag:false
      },{
        lng:121,lat:31,showFlag:false
      }],
      data: [
        // { lng: 116.418261, lat: 39.921984, count: 50 },
        // { lng: 116.423332, lat: 39.916532, count: 51 },
        // { lng: 116.419787, lat: 39.930658, count: 15 }
      ],
      mapStyle: {

        styleJson: [
          {
            "featureType": "land",
            "elementType": "geometry",
            "stylers": {
              "color": "#212121"
            }
          },
          {
            "featureType": "building",
            "elementType": "geometry",
            "stylers": {
              "color": "#2b2b2b"
            }
          },
          {
            "featureType": "highway",
            "elementType": "all",
            "stylers": {
              "lightness": -42,
              "saturation": -91
            }
          },
          {
            "featureType": "arterial",
            "elementType": "geometry",
            "stylers": {
              "lightness": -77,
              "saturation": -94
            }
          },
          {
            "featureType": "green",
            "elementType": "geometry",
            "stylers": {
              "color": "#1b1b1b"
            }
          },
          {
            "featureType": "water",
            "elementType": "geometry",
            "stylers": {
              "color": "#181818"
            }
          },
          {
            "featureType": "subway",
            "elementType": "geometry.stroke",
            "stylers": {
              "color": "#181818"
            }
          },
          {
            "featureType": "railway",
            "elementType": "geometry",
            "stylers": {
              "lightness": -52
            }
          },
          {
            "featureType": "all",
            "elementType": "labels.text.stroke",
            "stylers": {
              "color": "#313131"
            }
          },
          {
            "featureType": "all",
            "elementType": "labels.text.fill",
            "stylers": {
              "color": "#8b8787"
            }
          },
          {
            "featureType": "manmade",
            "elementType": "geometry",
            "stylers": {
              "color": "#1b1b1b"
            }
          },
          {
            "featureType": "local",
            "elementType": "geometry",
            "stylers": {
              "lightness": -75,
              "saturation": -91
            }
          },
          {
            "featureType": "subway",
            "elementType": "geometry",
            "stylers": {
              "lightness": -65
            }
          },
          {
            "featureType": "railway",
            "elementType": "all",
            "stylers": {
              "lightness": -40
            }
          },
          {
            "featureType": "boundary",
            "elementType": "geometry",
            "stylers": {
              "color": "#8b8787",
              "weight": "1",
              "lightness": -29
            }
          }
        ]
      },
      center: { lng: 0, lat: 0 },
      zoom: 10
    }
  },
// {
//   "address": "北京市朝阳区XX小区",
//   "quantity": 10000,
//   "point": {
//   "lng": 116.42,
//   "lat": 40.002
// }
// },
  created() {
    //let temp=new Array();
    var list=new Array();
    var data = {};	// 创建了一个空对象


    for(var i=0;i<50;i++){
      let tlat=randomNum(31.3,30.9,5)
      let tlng=randomNum(121.8,121.4,5)
      var pos = {
        lng:tlng,
        lat:tlat
      };
      data = {
        point: pos,		// 创建了一个属性
        address:"test"
      }
      list.push(data);
      //console.error(data.point)
    }
    //console.error(JSON.stringify(list));
    // this.$ajax.get('/static/data.json').then(res => {
    //   //console.error(res.data)
    //   for (let i = 0; i < res.data.length; i++) {
    //     let x = res.data[i]
    //     //console.log(x)
    //     this.data.push({ lng: x.point.lng, lat: x.point.lat,count:100})
    //   }
    //   // console.log(this.data)
    //   // this.data = res.data
    // })
    this.getHeatData();
  },
  methods: {
    getHeatData() {
      const { index, size, key, selectUAVType } = this;
      let query = {
        key,
        start: this.queryStartTime,
        end: this.queryEndTime,
        uavModel: selectUAVType,
        desc: true,
      };
      http_request(
        request_his_heatMapData,
        null,
        (_data) => {
          //const { data } = _data;
          //console.log(JSON.stringify(_data)+"=======================");
          for (let i = 0; i < _data.length; i++) {
            let x = _data[i]
            //console.log(x)
            this.data.push({ lng: x.point.lng, lat: x.point.lat,count:10})
          }
          // this.tableData = data;
          // this.total = snumSize;
          // this.TrueOrFlase = true;
          // this.InitCheckData(false);
        }
      );
    },
    handler({ BMap, map }) {
      // console.log(BMap, map)
      this.center.lng = 121.653331
      this.center.lat = 31.139654
      this.zoom = 12
    }
  }
}
/***************************************
 * 生成从minNum到maxNum的随机数。
 * 如果指定decimalNum个数，则生成指定小数位数的随机数
 * 如果不指定任何参数，则生成0-1之间的随机数。
 *
 * @minNum：[数据类型是Integer]生成的随机数的最小值（minNum和maxNum可以调换位置）
 * @maxNum：[数据类型是Integer]生成的随机数的最大值
 * @decimalNum：[数据类型是Integer]如果生成的是带有小数的随机数，则指定随机数的小数点后的位数
 *
 ****************************************/
function randomNum(maxNum, minNum, decimalNum) {
  var max = 0, min = 0;
  minNum <= maxNum ? (min = minNum, max = maxNum) : (min = maxNum, max = minNum);
  switch (arguments.length) {
    case 1:
      return Math.floor(Math.random() * (max + 1));
      break;
    case 2:
      return Math.floor(Math.random() * (max - min + 1) + min);
      break;
    case 3:
      return (Math.random() * (max - min) + min).toFixed(decimalNum);
      break;
    default:
      return Math.random();
      break;
  }
}
</script>

<style scoped>
#container {
  width: 500px;
  height: 500px;
  /* border: #ccc solid 1px; */
}
.heatmap {
  width: 40%;
  height: 80%;
  position: fixed;
}

.ceter{
   color: rgba(52, 58, 63, 1);
   font-family: PingFang SC;
   font-weight: 600;
   font-size: 16px;
   text-align: center; /*水平居中 */
   line-height: 40px; /*垂直居中 */
 }

</style>

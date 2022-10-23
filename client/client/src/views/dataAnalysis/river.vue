<template>
  <div class="charts-border" style="height:400px;width:100%;">
    <div>当月无人机数量统计</div>
    <v-chart
      :options="option"
      :autoresize="true"
      theme="chartsTheme"
    />
  </div>
</template>
<script>

import { ChartLine, ChartBar, ChartPie } from "@/components/charts"; // 图表组件
import constData from "@/util/constData"; // 保存的常量
// 工具
import axios from "axios";
import {http_request, request_his_heatMapData, request_his_riverData} from "../../modes/api";
//import dayjs from "dayjs";

export default {
  name: "river",
  components: {},
  data() {
    return {

      data:[],
      option: {
        tooltip: {
          trigger: "axis",
          axisPointer: {
            type: "line",
            lineStyle: {
              color: "rgba(0,0,0,0.2)",
              width: 1,
              type: "solid"
            }
          }
        },

        legend: {
          data: ["中银大厦", "迪士尼", "足球场"]
        },
        singleAxis: {
          top: 150,
          bottom: 50,
          axisTick: { // 坐标轴刻度相关设置。
            alignWithLabel: true, // 类目轴中在 boundaryGap 为 true 的时候有效，可以保证刻度线和标签对齐。
            inside: false, // 坐标轴刻度是否朝内，默认朝外。
            length: 5, // 坐标轴刻度的长度。
            lineStyle: { // 刻度线的样式。
              color: 'white',
              width: 2
            }
          },
          axisLabel: { // 坐标轴刻度标签的相关设置。
            textStyle: { // 写在外面没效果
              color: 'white',
              fontSize: '14'
            }
          },
          type: "time",
          splitLine: {
            show: true,
            lineStyle: {
              type: "dashed",
              opacity: 0.9
            }
          },
          axisLine: { // 坐标轴轴线相关设置。
            show: true, // 是否显示坐标轴
            // symbol: ['none', 'arrow'], // 坐标轴的箭头
            lineStyle: {
              color: 'white',
              width: '2',
              type: 'solid' // 坐标轴的类型，solid、dashed、dotted
            }
          },

        },
        series: [
          {
            type: "themeRiver",
            itemStyle: {
              emphasis: {
                shadowBlur: 20,
                shadowColor: "rgba(0, 0, 0, 0.8)"
              }
            },
            label: {
              color: "white",
              position: "top",
              fontSize: 14,
            },
            data:this.data
            //data:[["2022/9/19","149","文化公园"],["2022/9/20","283","文化公园"],["2022/9/21","357","文化公园"],["2022/9/22","316","文化公园"],["2022/9/23","299","文化公园"],["2022/9/24","525","文化公园"],["2022/9/25","571","文化公园"],["2022/9/26","14","文化公园"],["2022/9/27","6","文化公园"],["2022/9/29","37","文化公园"],["2022/9/30","466","文化公园"],["2022/10/1","287","文化公园"],["2022/10/2","92","文化公园"],["2022/10/3","29","文化公园"],["2022/10/4","10","文化公园"],["2022/10/5","8","文化公园"],["2022/9/19","179","中银"],["2022/9/20","313","中银"],["2022/9/21","387","中银"],["2022/9/22","346","中银"],["2022/9/23","329","中银"],["2022/9/24","555","中银"],["2022/9/25","601","中银"],["2022/9/26","44","中银"],["2022/9/27","36","中银"],["2022/9/29","67","中银"],["2022/9/30","496","中银"],["2022/10/1","317","中银"],["2022/10/2","122","中银"],["2022/10/3","59","中银"],["2022/10/4","40","中银"],["2022/10/5","38","中银"],["2022/9/19","159","世博园"],["2022/9/20","293","世博园"],["2022/9/21","367","世博园"],["2022/9/22","326","世博园"],["2022/9/23","309","世博园"],["2022/9/24","535","世博园"],["2022/9/25","581","世博园"],["2022/9/26","24","世博园"],["2022/9/27","16","世博园"],["2022/9/29","47","世博园"],["2022/9/30","476","世博园"],["2022/10/1","297","世博园"],["2022/10/2","102","世博园"],["2022/10/3","39","世博园"],["2022/10/4","20","世博园"],["2022/10/5","18","世博园"]]
          }
        ]
      }
    };
  },
  watch: {},
  created() {
    this.getRiverData();
  },
  mounted() {},
  methods: {
    getRiverData() {

      http_request(
        request_his_riverData,
        null,
        (_data) => {
          //const { data } = _data;

          this.data=_data;
          console.log(JSON.stringify(this.data)+"=======================");
        }
      );
    },

  }
};
</script>

<style lang="less">
</style>



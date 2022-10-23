<template>
  <div ref="chart" style="width: 180px; height: 180px"></div>
</template>

<script>
export default {
  name: "Dashbord"
}

let echarts = require("echarts");
drawMyChart(){
  let myChart = echarts.init(this.$refs.chart);
  // var myChart = echarts.init(document.getElementById("chart"));
  // 绘制图表
  myChart.setOption({
    tooltip: {
      formatter: "{a} <br/>{b} : {c}%",
    },
    toolbox: {
      show: false,
      feature: {
        restore: {},
        saveAsImage: {},
      },
    },
    // 配置
    series: [
      {
        name: "CPU使用率",
        type: "gauge",
        axisLabel: {distance: 0, fontWeight: 100, shadowOffsetY: 100, height: 100},
        axisLine: {
          lineStyle: {
            width: 10 // 修改仪表盘宽度的属性
          }
        },
        splitLine: {//分隔线样式相关
          length: 15,//分割线的长度
          lineStyle: {
            width: 1,
            color: '#b0b3b8'
          }
        },
        title: {offsetCenter: [0, '85%'], color: 'red'},
        detail: {formatter: "{value}%", offsetCenter: [0, '65%'], fontStyle: "oblique", fontSize: 12},
        pointer: {width: 4},
        data: [{value: 13.33, name: "CPU使用率"}]
      },
    ],
  });
},
</script>


<style scoped>

</style>

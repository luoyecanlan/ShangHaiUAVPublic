import {
  Style,
  Stroke,
  Fill,
  Icon,
  Image,
  Text,
  RegularShape,
  Circle as CircleStyle
} from "ol/style";

import { GetTargetAlarmColor } from "../../../static/css/GeneralStyle";
import OlStyleIcon from "ol/style/Icon";
import OlStyleStroke from "ol/style/Stroke";
import OlStyleFill from "ol/style/Fill";

export function GetTGSectorSelectStyle(colorFill, colorTans) {
  let iconFeatureStyle = new Style({
    fill: new Fill({
      color: colorTans
    }),
    stroke: new Stroke({
      color: colorFill,
      width: 6,
    })
  });

  return iconFeatureStyle;
}

export function GetTGSectorTextStyle(id, colorFill) {

  let StylyText = new Style({
    text: new Text({
      textAlign: 'left',
      text: id,
      font: '16px Microsoft YaHei',
      backgroundStroke: new OlStyleStroke({
        color: '#FFFFFF',
        width: 1
      }),
      //标签的背景填充
      backgroundFill: new OlStyleFill({
        color: colorFill
      }),
      fill: new OlStyleFill({
        color: '#FFFFFF',
      })
    })
  })

  return StylyText;
}

export function GetTGSectorStyle(colorFill, colorTans) {
  let iconFeatureStyle = new Style({
    fill: new Fill({
      color: colorTans
    }),
    stroke: new Stroke({
      color: colorFill,
      width: 2,
    })
  });

  return iconFeatureStyle;
}


//获取目标的 跟踪 转发 打击 的SVG图片资源
export function GetTGIMGText(tgImgText, color) {
  //状态相关  待修改
  return new Style({
    zIndex: 999,
    text: new Text({
      font: "14px blueengine-font",
      text: tgImgText,
      textAlign: "right",
      offsetX: -23,
      backgroundStroke: new Stroke({
        color: color,
        width: 1
      }),
      padding: [4, -5, 0, 5],
      fill: new Fill({
        color: "rgba(255,255,255,1)"
      }),
      backgroundFill: new Fill({
        color: color,
      })
    })
  });
}


// 获取默认目标的选中外框样式
export function GetTGSelectStyle(threat, color) {
  return [
    new Style({
      zIndex: 999,
      image: new CircleStyle({
        radius: 35,
        stroke: new Stroke({
          color: color, // red
          width: 5,
          lineDashOffset: -8,
          lineDash: [20, 16.58],
          lineCap: 'square'
        })
      })
    }),
    new Style({
      zIndex: 999,
      image: new CircleStyle({
        radius: 30,
        stroke: new Stroke({
          color: GetTargetAlarmColor(threat, 0.4), // white
          width: 5
        })
      })
    })
  ]
}


export function GetWarnningZoneStyle(_info) {
  //console.log('info=====',_info)
  const { name, fullColor, lineColor, lineThickness } = _info;

  return [
    new Style({
      fill: new Fill({
        color: fullColor
      }),
      stroke: new Stroke({
        color: lineColor,
        width: lineThickness,
        lineDash:[4,8]
      })
    }),
    // new Style({
    //   text: new Text({
    //     font: '24px sans-serif',
    //     text: name,
    //     fill: new Fill({
    //       color: [51, 51, 51, 1]
    //     })
    //   })
    // })
  ]
}

/**
 * 获取目标的默认样式 [未选中]
 * @param id
 * @param color
 * @param threat
 * @returns {Array}
 */
export function GetTGUsuallyStyle(id, color, threat) {
  let Styles = [];
  Styles.push
    (
      new Style({
        zIndex: 500,
        text: new Text({
          font: "14px Microsoft YaHei",
          text: ' ' + id,
          textAlign: "left",
          offsetX: 30,
          backgroundStroke: new Stroke({
            color: color,
            width: 1
          }),
          padding: [4, 5, 0, 5],
          fill: new Fill({
            color: "#FFFFFF"//#FFFFFF
          }),
          backgroundFill: new Fill({
            color: color,
          })
        })
      }),
      new Style({
        zIndex: 999,
        image: new CircleStyle({
          radius: 26,
          fill: new Fill({
            color: "#FFFFFF"
          }),
          stroke: new Stroke({
            color: color,
            width: 4
          })
        })
      }),
      //二层圆
      new Style({
        zIndex: 999,
        image: new CircleStyle({
          radius: 22,
          fill: new Fill({
            color: GetTargetAlarmColor(threat, 0.6)
          }),
          stroke: new Stroke({
            color: GetTargetAlarmColor(threat, 0.6),
            width: 2
          })
        })
      }),
      //中心圆
      new Style({
        zIndex: 999,
        image: new CircleStyle({
          radius: 18,
          fill: new Fill({
            color: GetTargetAlarmColor(threat, 0.8)
          })
        })
      }),
      //无人机图标
      new Style({
        zIndex: 999,
        image: new OlStyleIcon({
          src: "/static/img/tg/uav.png",
          offset: [-1, -1]
        })
      }));

  return Styles;
}


export function Image_Style(src, img) {
  return [
    new Style({
      image: new Icon({
        anchor: [0.5, 0.5],
        // anchorOrigin: 'bottom-right',
        crossOrigin: "anonymous",
        src: src,
        img: img,
        imgSize: img ? [img.width, img.height] : undefined
      })
    }),
    new Style({
      image: new CircleStyle({
        radius: 2,
        fill: new Fill({
          color: [255, 0, 0, 1]
        })
      })
    })
  ];
}

export function Select_Style() {
  return new Style({
    stroke: new Stroke({
      width: 2,
      color: [0, 255, 0, 1]
    }),
    fill: new Fill({
      color: [0, 0, 0, 0.5]
    })
  });
}
export function None_Style() {
  return new Style({
    stroke: new Stroke({
      color: [0, 0, 0, 0]
    }),
    fill: new Fill({
      color: [0, 0, 0, 0]
    })
  });
}

export function Draw_Zone_Style() {
  return [
    new Style({
      fill: new Fill({
        color: "rgba(127,255,206,0.5)"
      }),
      stroke: new Stroke({
        color: "#EE1A48",
        width: 1
      })
    })
  ];
}

export function Zone_Style() {
  return [
    new Style({
      fill: new Fill({
        color: 'rgba(200,200,200,0.5)'
      }),
      stroke: new Stroke({
        color: '#333333',
        width: 0.5
      })
    })
  ];
}

export function GetFlyerStyle(id) {
  let color = "rgb(117, 208, 48 , 1)" //绿色
  let Styles = [];
  Styles.push
    (
      new Style({
        zIndex: 500,
        text: new Text({
          font: "14px Microsoft YaHei",
          text: ' ' + id,
          textAlign: "left",
          offsetX: 30,
          backgroundStroke: new Stroke({
            color: color,
            width: 1
          }),
          padding: [4, 5, 0, 5],
          fill: new Fill({
            color: "#FFFFFF"//#FFFFFF
          }),
          backgroundFill: new Fill({
            color: color,
          })
        })
      }),
      new Style({
        zIndex: 999,
        image: new CircleStyle({
          radius: 26,
          fill: new Fill({
            color: "#FFFFFF"
          }),
          stroke: new Stroke({
            color: color,
            width: 4
          })
        })
      }),
      //二层圆
      new Style({
        zIndex: 999,
        image: new CircleStyle({
          radius: 22,
          fill: new Fill({
            color: "rgb(117, 208, 48 , 0.6)"
          }),
          stroke: new Stroke({
            color: "rgb(117, 208, 48 , 0.6)",
            width: 2
          })
        })
      }),
      //中心圆
      new Style({
        zIndex: 999,
        image: new CircleStyle({
          radius: 18,
          fill: new Fill({
            color: "rgb(117, 208, 48 , 0.8)",
          })
        })
      }),
      //无人机图标
      new Style({
        zIndex: 999,
        image: new OlStyleIcon({
          src: "/static/img/tg/flyer.png",
          offset: [-1, -1]
        })
      }));

  return Styles;
}

export function Warnning_Zone_Style(_info) {
  //console.log('info=====',_info)
  const { name, fullColor, lineColor, lineThickness } = _info;
  return [
    new Style({
      fill: new Fill({
        color: fullColor
      }),
      stroke: new Stroke({
        color: lineColor,
        width: lineThickness
      })
    }),
    new Style({
      text: new Text({
        font: '24px sans-serif',
        text: name,
        fill: new Fill({
          color: [51, 51, 51, 1]
        })
      })
    })
  ]
}

export function GetFeatureStyle__Dev(id, ImageSrc, offestX) {
  let Styles = [];
  // Styles.push(
  //   new Style({
  //     image: new CircleStyle({
  //       radius: 6,
  //       stroke:new Stroke({
  //         width:1,
  //         color:'#333333'
  //       }),
  //       fill: new Fill({
  //         color: '#43DCFF',
  //       }),
  //     }),
  //   })
  // )
  //console.error(ImageSrc+"99999999999999999999999999999999999999999999999999999999999999999999")
  // 图标   暂时去掉
  Styles.push(
    new Style({
      zIndex: 999,
      //@click=onhover,
      image: new OlStyleIcon({
        src: ImageSrc,

        // anchor: [-offestX, 0.5],
        // anchorXUnits: 'pixels', //锚点X值单位
        // anchorYUnits: 'fraction' //锚点Y值单位
      })
    })
  )

  // 设置style
  // Styles.push(
  //   new Style({
  //     zIndex: 400,
  //     text: new Text({
  //       textAlign: 'left',
  //       text: ' ' + id,
  //       font: '16px Microsoft YaHei',
  //       offsetX: offestX + 30,
  //       offsetY: 75,
  //       backgroundStroke: new OlStyleStroke({
  //         color: '#42DCFD',
  //         width: 1
  //       }),
  //       //标签的背景填充
  //       backgroundFill: new OlStyleFill({
  //         color: '#0F152F'
  //       }),
  //       fill: new OlStyleFill({
  //         color: '#42DCFD',
  //       })
  //     })
  //   })
  // )
  return Styles
  // let Styles = [];
  //
  // Styles.push(
  //   new Style({
  //     zIndex:999,
  //     image: new OlStyleIcon({
  //       src: ImageSrc,
  //     })
  //   })
  // )
  //
  // // 设置style
  // Styles.push(
  //   new Style({
  //     zIndex:400,
  //     text: new Text({
  //       textAlign: 'left',
  //       text: ' '+id,
  //       font: '16px Microsoft YaHei',
  //       offsetX: -47,
  //       offsetY:75,
  //       backgroundStroke:new OlStyleStroke({
  //         color:'#42DCFD',
  //         width:1
  //       }),
  //       //标签的背景填充
  //       backgroundFill:new OlStyleFill({
  //         color:'#0F152F'
  //       }),
  //       fill: new OlStyleFill({
  //         color: '#42DCFD',
  //       })
  //     })
  //   })
  // )
  //
  //
  // return Styles
}

export function Sharp_Style() {
  return [
    new Style({
      image: new CircleStyle({
        radius: 6,
        stroke: new Stroke({
          width: 1,
          color: [76, 145, 225, 1]
        }),
        fill: new Fill({
          color: [76, 208, 69, 0.5]
        })
      })
    })
  ];
}

/**
 * 获取文本样式
 */
export const GetEquipLineTextStyle = (text, size = 14, offsetY = 16) => {
  return new Style({
    text: new Text({
      text,
      font: `${size}px sans-serif`,
      fill: new Fill({
        color: [53, 53, 53, .3]
      }),
      placement: 'line',
      rotation: 45,
      rotateWithView: true,
      textAlign: 'right',
      textBaseline: 'middle'
    })
  });
};

/**
 * 地图元素颜色
 */
export const mapColor = {
  service_top: [153, 153, 153, 0.6],
  running: [27, 168, 189, 0.6],
  stop: [0, 255, 0, 0.6],
  error: [255, 107, 113, 0.6],
  monitor: [27, 168, 189, 0.6],
  jam: [27, 168, 189, 0.6],
  guidance: [27, 168, 189, 0.6],
  normal: [102, 102, 102],
  target: [20, 205, 176, 1],
  target_1: [27, 168, 189, 1],
  target_2: [255, 202, 0, 1],
  target_3: [255, 107, 113, 1],
  target_hover: [20, 205, 176, 0.2],
  target_hover_1: [27, 168, 189, 0.5],
  target_hover_2: [255, 202, 0, 0.2],
  target_hover_3: [255, 107, 113, 0.2],
  rang_black: [255, 255, 255, 0.2],
  rang_black_stroke: [0, 0, 0, 0.2],
  rang_blue: [0, 134, 200, 0.2],
  rang_blue_stroke: [0, 134, 200, 0.3],
  rang_red: [255, 107, 113, 0.2],
  rang_red_stroke: [255, 107, 113, 0.3],
  selected_color: [255, 0, 0, 1]
};

//设备等距线样式
export const EquipmentLineStyle = {
  line_red: new Style({
    stroke: new Stroke({
      lineDash: [3, 3],
      color: mapColor.rang_red_stroke
    })
  }),
  line_black: new Style({
    stroke: new Stroke({
      lineDash: [3, 3],
      color: mapColor.rang_black_stroke
    })
  }),
  line_blue: new Style({
    stroke: new Stroke({
      lineDash: [3, 3],
      color: mapColor.rang_blue_stroke
    })
  })
};
let onHover=function (info) {
  // this.hover_device = info;
  // const { id } = info;
  // let _item = this.$refs[id][0];
  // this.infoOffsetLeft = _item.offsetLeft + _item.offsetWidth / 2;
  alert("tttttttttttttttt");
};
let onLeave=function() {
  this.hover_device = undefined;
}






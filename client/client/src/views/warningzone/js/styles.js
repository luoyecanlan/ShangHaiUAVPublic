import {
  Style,
  Stroke,
  Fill,
  Icon,
  Image,
  Text,
  RegularShape,
  Circle,
  Circle as CircleStyle
} from "ol/style";

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
      image: new Circle({
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

export function Warnning_Zone_Style(_info) {
  //console.log('info=====',_info)
  const {name, fullColor, lineColor, lineThickness} = _info;
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

export function Text_Info_Style(_name,_draw) {
  return [
    _draw ? Draw_Zone_Style()[0] : Zone_Style()[0],
    new Style({
      text: new Text({
        font: '24px sans-serif',
        text: _name,
        fill: new Fill({
          color: _draw ? [29, 165, 122, 0.8] : [51,51,51, 0.5]
        })
      })
    })
  ]
}

export function Sharp_Style() {
  return [
    new Style({
      image: new Circle({
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
  // return [new Style({
  //   stroke: new Stroke({ color: [255,0,0,0.3], width: 2 }),
  //   fill: new Fill({ color: [255,0,0,0.3]}),
  //   zIndex: -1
  // }),
  // new Style({
  //   image: new RegularShape({
  //     radius: 10, radius2: 5, points: 5, fill: new Fill({
  //       color: [255, 0, 0, 0.5]
  //     })
  //   }),
  //   stroke: new Stroke({
  //     color: [255, 0, 0, 1],
  //     width: 1
  //   }),
  //   fill: new Fill({
  //     color: [255, 0, 0, 0.5],
  //   })
  // })]
}

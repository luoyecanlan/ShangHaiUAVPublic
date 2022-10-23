import Draw from 'ol/interaction/Draw'
import VectorSource from 'ol/source/Vector';
import VectorLayer from 'ol/layer/Vector';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';

import {
  unByKey
} from 'ol/Observable.js';
import Overlay from 'ol/Overlay';
import {
  getArea,
  getLength
} from 'ol/sphere.js';
import View from 'ol/View';
import {
  LineString,
  Polygon
} from 'ol/geom.js';
import {
  Circle as CircleStyle,
  Fill,
  Stroke,
  Style
} from 'ol/style.js';
export default{

  measure(map, measureType) {
    /**
     * Currently drawn feature.
     * @type {module:ol/Feature~Feature}
     */
    let sketch;


    /**
     * The help tooltip element.
     * @type {Element}
     */
    let helpTooltipElement;


    /**
     * Overlay to show the help messages.
     * @type {module:ol/Overlay}
     */
    let helpTooltip;


    /**
     * The measure tooltip element.
     * @type {Element}
     */
    let measureTooltipElement;


    /**
     * Overlay to show the measurement.
     * @type {module:ol/Overlay}
     */
    let measureTooltip;


    /**
     * Message to show when the user is drawing a polygon.
     * @type {string}
     */
    let continuePolygonMsg = '继续点击绘制多边形';


    /**
     * Message to show when the user is drawing a line.
     * @type {string}
     */
    let continueLineMsg = '继续点击绘制线';

    createMeasureTooltip();
    createHelpTooltip();

    /**
     * Handle pointer move.
     * @param {module:ol/MapBrowserEvent~MapBrowserEvent} evt The event.
     */
    let pointerMoveHandler = function (evt) {
      if (evt.dragging) {
        return;
      }
      /** @type {string} */
      let helpMsg = '请点击开始绘制';

      if (sketch) {
        let geom = (sketch.getGeometry());
        if (geom instanceof Polygon) {
          helpMsg = continuePolygonMsg;
        } else if (geom instanceof LineString) {
          helpMsg = continueLineMsg;
        }
      }

      helpTooltipElement.innerHTML = helpMsg;
      helpTooltip.setPosition(evt.coordinate);

      helpTooltipElement.classList.remove('hidden');
    };

    map.on('pointermove', pointerMoveHandler);

    map.getViewport().addEventListener('mouseout', function () {
      helpTooltipElement.classList.add('hidden');
    });

    let draw;
    let formatLength = function (line) {
      let length = getLength(line);
      let output;
      if (length > 100) {
        output = (Math.round(length / 1000 * 100) / 100) +
          ' ' + 'km';
      } else {
        output = (Math.round(length * 100) / 100) +
          ' ' + 'm';
      }
      return output;
    };
    let formatArea = function (polygon) {
      let area = getArea(polygon);
      let output;
      if (area > 10000) {
        output = (Math.round(area / 1000000 * 100) / 100) +
          ' ' + 'km<sup>2</sup>';
      } else {
        output = (Math.round(area * 100) / 100) +
          ' ' + 'm<sup>2</sup>';
      }
      return output;
    };
    let source;
    // let layer ;
    // 获取存放feature的vectorlayer层。map初始化的时候可以添加好了
    for(let layerTmp of map.getLayers().getArray()){
      if(layerTmp.get("name")=="feature"){
        // layer = layerTmp;
        // layerTmp.setSource(null)
        source= layerTmp.getSource();
      }
    }

    function addInteraction() {
      let type = (measureType == 'area' ? 'Polygon' : 'LineString');
      draw = new Draw({
        source: source,
        type: type,
        style: new Style({
          fill: new Fill({
            color: 'rgba(255, 255, 255, 0.2)'
          }),
          stroke: new Stroke({
            color: 'rgba(0, 0, 0, 0.5)',
            lineDash: [10, 10],
            width: 2
          }),
          image: new CircleStyle({
            radius: 5,
            stroke: new Stroke({
              color: 'rgba(0, 0, 0, 0.7)'
            }),
            fill: new Fill({
              color: 'rgba(255, 255, 255, 0.2)'
            })
          })
        })
      });
      map.addInteraction(draw);

      let listener;
      draw.on('drawstart',
        function (evt) {
          // set sketch
          sketch = evt.feature;

          /** @type {module:ol/coordinate~Coordinate|undefined} */
          let tooltipCoord = evt.coordinate;

          listener = sketch.getGeometry().on('change', function (evt) {
            let geom = evt.target;
            let output;
            if (geom instanceof Polygon) {
              output = formatArea(geom);
              tooltipCoord = geom.getInteriorPoint().getCoordinates();
            } else if (geom instanceof LineString) {
              output = formatLength(geom);
              tooltipCoord = geom.getLastCoordinate();
            }
            measureTooltipElement.innerHTML = output;
            measureTooltip.setPosition(tooltipCoord);
          });
        }, this);

      draw.on('drawend',
        function () {
          measureTooltipElement.className = 'tooltip tooltip-static';
          measureTooltip.setOffset([0, -7]);
          // unset sketch
          sketch = null;
          // unset tooltip so that a new one can be created
          measureTooltipElement = null;
          createMeasureTooltip();
          unByKey(listener);
          map.un('pointermove', pointerMoveHandler);
          map.removeInteraction(draw);
          helpTooltipElement.classList.add('hidden');
        }, this);
    }

    function createHelpTooltip() {
      if (helpTooltipElement) {
        helpTooltipElement.parentNode.removeChild(helpTooltipElement);
      }
      helpTooltipElement = document.createElement('div');
      helpTooltipElement.className = 'tooltip hidden';
      helpTooltip = new Overlay({
        element: helpTooltipElement,
        offset: [15, 0],
        positioning: 'center-left'
      });
      map.addOverlay(helpTooltip);
    }

    function createMeasureTooltip() {
      if (measureTooltipElement) {
        measureTooltipElement.parentNode.removeChild(measureTooltipElement);
      }
      measureTooltipElement = document.createElement('div');
      measureTooltipElement.className = 'tooltip tooltip-measure';
      measureTooltip = new Overlay({
        element: measureTooltipElement,
        offset: [0, -15],
        positioning: 'bottom-center'
      });
      map.addOverlay(measureTooltip);
    }
    // 量测调用
    addInteraction();
  }
}

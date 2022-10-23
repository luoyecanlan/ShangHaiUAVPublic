import XYZ from 'ol/source/XYZ'
import OSM from 'ol/source/OSM'
import BingMap from 'ol/source/BingMaps'


/**
 * OSM Source
 * @returns {OSM}
 * @constructor
 */
export function OSM_Source()
{
  return new OSM()
}

/**
 * XYZ Source
 * @returns {XYZ}
 * @constructor
 */
export function XYZ_Source() {
  const tilewidth = 511;
  const tileheight = 256;
  const pxwidth = 6744 + 5722;
  const pxheight = 3372 + 2321;
  return  new XYZ({
    tileUrlFunction: function (xyz, obj1, obj2) {
      //debugger
      let x = xyz[1]
      let y = Math.abs(xyz[2])
      let z = xyz[0];
      let _z = z - 13
      let _s = Math.pow(2, _z)
      let _x = (x - tilewidth * _s) - pxwidth * ((_z - 1) * 2)
      let _y = (y - tileheight * _s) - pxheight * ((_z - 1) * 2)
      if (_z == 1) {
        _x = (x - tilewidth * _s) - pxwidth
        _y = (y - tileheight * _s) - pxheight
      } else if (_z == 2) {
        _x = (x - tilewidth * _s) - pxwidth * ((_z - 1) * 2)
        _y = (y - tileheight * _s) - pxheight * ((_z - 1) * 2)
      } else if (_z == 3) {
        _x = (x - tilewidth * _s) - pxwidth * ((_z - 1) * 2)
        _y = (y - tileheight * _s) - pxheight * ((_z - 1) * 2)
      } else if (_z == 4) {
        _x = (x - tilewidth * _s) - pxwidth * ((_z) * 2)
        _y = (y - tileheight * _s) - pxheight * ((_z) * 2)
      } else if (_z == 5) {
        _x = (x - tilewidth * _s) - pxwidth * ((_z + 3) * 2)
        _y = (y - tileheight * _s) - pxheight * ((_z + 3) * 2)
      } else if (_z == 6) {
        _x = (x - tilewidth * _s) - pxwidth * ((_z + 10) * 2)
        _y = (y - tileheight * _s) - pxheight * ((_z + 10) * 2)
      } else if (_z == 7) {
        _x = (x - tilewidth * _s) - pxwidth * ((_z + 25) * 2)
        _y = (y - tileheight * _s) - pxheight * ((_z + 25) * 2)
      }
      let a = 'http://oss.lishi-cloud.com/map/liangshuihe3/map' + _z + '/' + _x + ',' + _y + '.jpg'
      return a
    }
  });
}

/**
 * Bing Map Source
 * @param key
 * @returns {BingMaps}
 * @constructor
 */
export function BingMap_Source(key='Am9Jh-tsLgampLiX0yGDc4V8DWzQQ6gl5fIn8WvbOpLTCYTEzGopyBArdiRLlf20') {
  return new BingMap({
    key,
    imagerySet:'AerialWithLabels',
    maxZoom:19
  })
}

import Feature from 'ol/Feature'
import Point from 'ol/geom/Point'
import {fromLonLat} from 'ol/proj'
import {Sharp_Style} from './styles'

export function Package_Feature(info){
  let _f= new Feature({
    geometry:new Point(
      fromLonLat(info.position)
    )
  })
  _f.setStyle(Sharp_Style())
  _f.setId(info.id)
  return _f;
}

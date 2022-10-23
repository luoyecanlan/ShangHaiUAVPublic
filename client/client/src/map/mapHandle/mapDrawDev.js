import {Get_Dev_Layer} from "../js";
import {
  Draw_Dev_Icon_Marker, Draw_Warning_RoundMarker,
  Draw_Warning_Marker_Line, Draw_Target_SectorMarker, Draw_Position_Main_Icon_Marker, Draw_Position_Device_Icon_Marker
} from "./mapDraw";
import {
  RADAR_IMG,
  PINPU_IMG,
  GUANGDIAN_IMG,
  ATTACK_IMG,
  DECOY_IMG,
  RADAR_SELECT_IMG,
  PINPU_SELECT_IMG,
  GUANGDIAN_SELECT_IMG,
  ATTACK_SELECT_IMG,
  DECOY_SELECT_IMG,
  RADAR_ICON,
  GUANGDIAN_ICON, ATTACK_ICON, DECOY_ICON, YUNSHAO_ICON, PINPU_ICON, Main_ICON,
}
  from "../mapHandle/mapImgSource"
import {featureType_enum} from "../mapHandle/mapDraw";
import {forEach} from "lodash";

let dev4offet = [];


export function Draw_DevListIcon_Marker(devList)
{
  if (null == devList)
  {
    return;
  }

  let yunshaoList=new Array();

  devList.forEach(info=>{
    if(info.category===10401){
      yunshaoList.push(info);
    }
    return yunshaoList;
  })

  if(dev4offet.length<devList.length){

    let devoffsetX = -60;
    for(let i = 0; i < devList.length; i++){
      devList[i].offestX = devoffsetX;
      devoffsetX = devoffsetX + 60;
    }
    dev4offet = devList;
  }
  else{
    for(let i = 0; i < devList.length; i++){
      let devinfo = dev4offet.filter(inf=>{
        if(inf.id == devList[i].id){
          return inf;
        }
      });
      if(devinfo!= undefined&&devinfo!=null&&devinfo.length!=0){
        devList[i].offestX = devinfo[0].offestX;
      }
    }
  }
  //let yunshaoLat=0,yunshaoLng=0;

  let devsinfo=new Array(devList.length);       ;
  devList.forEach((info,index) => {
    const {
      name, display, category, ip, port,
      lip, lport, lat, lng, alt,
      coverR, coverS, coverE, id,
      rectifyAZ, rectifyEl,
      threadAssessmentCount,
      targetTimeOut,
      statusReportingInterval,
      probeReportingInterval
    } = info;


    let feature_ = Get_Dev_Layer().getSource().getFeatureById(id);

    if (null == feature_)
    {

      if(category!=null)
      {
        if(category<=20000)
        {
          if(category>10200&&category<10400)
          {
            info.devImg = PINPU_ICON+'0.png';
          }else if(category>10400){
            info.devImg = YUNSHAO_ICON+'0.png';
          }
          else {
            info.devImg = RADAR_ICON+'0.png';
          }
        }
        else if(category>20000 && category<=30000)
        {
          info.devImg = GUANGDIAN_ICON+'0.png';
        }
        else if(category>30000 && category<=30500)
        {
          info.devImg = ATTACK_ICON + '0.png';
        }
        else
        {
          info.devImg = RADAR_ICON+'0.png';
        }

        //console.error("99"+name+info.devImg+info);
        //当设备为云哨时，首先确定阵地中心位置，并画出图标，然后画出云哨周围四个设备的图标（也就是当category为10401时需要先画个主图标再画个云哨图标

        for(var yunshao of yunshaoList) {
          let yunshaolat = yunshao.lat
          let yunshaolng = yunshao.lng

          //console.error("云哨坐标为:"+yunshao.name);
          if (info.display === yunshao.display) {


            if (info.category === 10401) {
              Draw_Position_Main_Icon_Marker("阵地标识", {
                lat: yunshaolat,
                lng: yunshaolng
              }, Main_ICON, info, featureType_enum.dev);
            }
            let angle = 360.0 / devList.length*yunshaoList.length;
            //console.error("ICON:"+angle+"-------------"+index+info.name+info.display);
            let resultangle = angle * index*Math.PI/180;
            //console.error("---"+angle+index);
            let firstdevlng = yunshaolng + 0.01 * Math.sin(resultangle),
              firstdevlat = yunshaolat + 0.01 * Math.cos(resultangle);
            Draw_Position_Device_Icon_Marker(info.name, {
              lat: firstdevlat,
              lng: firstdevlng
            }, info.devImg, info, featureType_enum.dev);
          }
        }
      }
    }
  })
}

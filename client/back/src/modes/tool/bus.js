import Vue from 'vue'

export const event_type={
  loading:'LOADING',
  begin_draw_zone:'BEGIN_DRAW_ZONE',
  init_zone_data:'INIT_ZONE_DATA',
  reload_zone_data:'RELOAD_ZONE_DATA',
  select_zone_change:'SELECT_ZONE_CHANGE'
}

const Bus=new Vue();

export default Bus;

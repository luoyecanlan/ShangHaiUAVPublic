<template>
    <div class="switch-box" :class="isActive?'active':''" @click.stop="doSwitch">
      <div class="switch-silder"></div>
      <span :class="isActive?'black-color':''">{{YesString?YesString:'ON'}}</span>
      <span :class="!isActive?'black-color':''">{{NoString?NoString:'OFF'}}</span>
    </div>
</template>

<script>
    export default {
      name: "SwitchButton",
      data() {
        return {
          isActive: false
        }
      },
      methods:{
        doSwitch(){

          this.isActive=!this.isActive;
          this.$emit('onChange',this.isActive);
        }
      },
      props:['YesString','NoString','Active'],
      watch:{
        Active(){
          this.isActive=this.Active;
        }
      }
    }
</script>

<style scoped>
    .switch-box{
      height: 20px;
      padding: 0px 2px;
      border-radius: 3px;
      border: 1px solid rgba(64,204,249,0.4);
      position: relative;
      display: flex;
      flex-direction: row;
      cursor: pointer;
    }
    .switch-silder{
      position: absolute;
      height: 14px;
      width: 48%;
      border-radius: 2px;
      margin-top: 2px;
    }
    .switch-silder{
      /*right: 2px;*/
      left: 99%;
      transform: translateX(-100%);
      background: linear-gradient(#c7c7c7,#6f6f6f);
      transition: all 0.2s;
    }
    .active .switch-silder{
      left: 50%;
      background: linear-gradient(#8be9ff,#49ddff);
      transition: all 0.2s;
    }
  .switch-box span{
    line-height: 18px;
    text-align: center;
    font-size: 14px;
    flex: 1;
    color: rgba(64,204,249,0.4);
    z-index: 10;
  }
  span.black-color{
    color: #020e2c;
    transition: color 0.5s;
  }
</style>

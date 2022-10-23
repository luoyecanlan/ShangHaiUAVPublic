<template>
  <div class="list-box">
    <div class="list-title-box">
      <span class="bbox list-title" is-title style="width: 154px;"><img src="../../assets/table/title-arrow.png" alt="">  {{$t('m.usually')}}</span>
      <span class="bbox list-add" style="right:52px;width: 12px;"
            @mouseleave="showHelper=false" @mouseenter="showHelper=true">
        <span>?</span>
      </span>
    </div>
    <div class="list-container" style="padding-left:40px;padding-right:40px;">
      <div class="tb-border-lt"></div>
      <div class="tb-border-rt"></div>
      <div class="tb-border-rb" style="bottom: 30px;"></div>
      <div class="tb-border-lb" style="bottom: 30px;"></div>
      <div class="tb-border-lc"></div>
      <div class="tb-border-rc"></div>
      <div class="tb-border-c-line"></div>

      <!--      内容-->
      <div class="calc-row">
        <div class="calc-box">
          <!--        根据绝对位置计算相对位置-->
          <abs-to-rel/>
        </div>
        <div class="v-middle-line"></div>
        <div class="calc-box">
          <!--        根据相对位置计算绝对位置-->
          <rel-to-abs/>
        </div>
      </div>
      <div class="calc-row" style="padding-top: 40px;flex: 42">
        <div class="calc-box">
          <!--        计算高度-->
          <calc-height/>
        </div>
        <div class="v-middle-line" style="margin-top: 10px"></div>
        <div class="calc-box">
          <!--        格式转换-->
          <format-conversion/>
        </div>
      </div>
      <transition name="fade">
        <div class="helper-viewer" v-show="showHelper">
          <div class="doc-box">
            <!--<p>当在一个自定义组件上使用 class 属性时，这些 class 将被添加到该组件的根元素上面。这个元素上已经存在的 class 不会被覆盖。</p>-->
            <!--<p>当 v-bind:style 使用需要添加浏览器引擎前缀的 CSS 属性时，如 transform，Vue.js 会自动侦测并添加相应的前缀。</p>-->
          </div>
        </div>
      </transition>
    </div>
  </div>
</template>

<script>

  import AbsToRel from './AbsToRel'
  import RelToAbs from './RelToAbs'
  import CalcHeight from './CalcHeight'
  import FormatConversion from './FormatConversion'
    export default {
      name: "Common",
      data() {
        return {
          showHelper: false
        }
      },
      components: {
        AbsToRel,
        RelToAbs,
        CalcHeight,
        FormatConversion
      }
    }
</script>

<style scoped>
  .tb-border-c-line{
    height: 1px;
    background-color: rgba(64,204,249,0.6);
    position: absolute;
    z-index: 99;
    top: 50%;
    margin-top: 16px;
    margin-left: -30px;
    width: 1696px;
  }
  .calc-row{
    flex: 58;
    display: flex;
    flex-direction: row;
    margin: -10px 0;
  }
  .calc-box{
    flex: 1;
  }
  .list-add span{
    font-family: MF-Lihei;
    font-size: 15px;
  }
</style>

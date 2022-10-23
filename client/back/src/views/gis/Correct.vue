<template>
  <div class="list-box">
    <div class="list-title-box">
      <span class="bbox list-title" is-title style="width: 274px;"><img src="../../assets/table/title-arrow.png" alt="">  {{$t('m.calibrationNorth')}} </span>
      <span class="bbox list-add" style="right:52px;width: 12px;"
            @mouseleave="showHelper=false" @mouseenter="showHelper=true">
        <span>?</span>
      </span>
    </div>
    <div class="list-container" style="padding-left:40px;padding-right:40px;">
      <div class="tb-border-lt"><div></div></div>
      <div class="tb-border-rt"><div></div></div>
      <div class="tb-border-rb" style="bottom: 30px;"><div></div></div>
      <div class="tb-border-lb" style="bottom: 30px;"><div></div></div>

      <div class="correct-container">
        <div style="flex: 1;">
          <point-correct/>
        </div>
        <div class="v-middle-line" style="margin-top: 20px;"></div>
        <div style="flex: 1;">
          <relative-correct/>
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
  import RelativeCorrect from './RelativeCorrect'
  import PointCorrect from './PointCorrect'
    export default {
      name: "Correct",
      data() {
        return {
          showHelper: false
        }
      },
      components: {
        RelativeCorrect,
        PointCorrect
      }
    }
</script>

<style scoped>
.correct-container{
  position: relative;
  height: 100%;
  width: 100%;
  display: flex;
  flex-direction: row;
}
.list-add span{
  font-family: MF-Lihei;
  font-size: 15px;
}
</style>

<template>
    <div class="devs-box">
      <div class="devs-scroll-container">
        <ul id="scroll">
          <li v-for="(dev,key) in device_infos" :key="key">
            <device-box>
              <device-item :device="dev"/>
            </device-box>
          </li>
          <li v-if="nodevices" v-for="(item,key) in nodevices" :key="`no_${item}`">
            <device-box>
              <no-device/>
            </device-box>
          </li>
        </ul>
      </div>
      <div class="scroll-button-prev" :disabled="!canToPrev"
           :class="canToPrev?'':'is-disabled'"
           @click.prevent="scrollToRight">
        <img src="../../assets/home/scroll-prev.png" alt="">
      </div>
      <div class="scroll-button-next" :disabled="!canToNext"
           :class="canToNext?'':'is-disabled'" @click.prevent="scrollToLeft">
        <img src="../../assets/home/scroll-next.png" alt="">
      </div>
    </div>
</template>

<script>
  import {mapState} from 'vuex'
  import DeviceItem from './DeviceItem'
  import NoDevice from "../../components/NoDevice";
  import DeviceBox from "../../components/DeviceBox";
    export default {
      name: "ScrollViewer",
      data() {
        return {
          canScrollPrev: false,
          canScrollNext: true,
          scrollStep: 410,
          scrollBoxWidth: 1640,
          scrolling:false,
          scrollElementId: 'scroll'
        }
      },
      computed: {
        ...mapState(['device_infos', 'device_status']),
        canToNext() {
          if (this.device_infos&&this.device_infos.length <= 4) {
            return false;
          }
          return this.canScrollNext;
        },
        canToPrev() {
          if (this.device_infos&&this.device_infos.length <= 4) {
            return false;
          }
          return this.canScrollPrev;
        },
        nodevices() {
          // debugger
          if (this.device_infos) {
            let tempLen = this.device_infos.length;
            let _len = 4 - tempLen;
            return _len < 0 ? 0 : _len;
          }
          return 4;
        }
      },
      methods: {
        doScroll(scrollId, step, callback) {
          this.scrolling=true;
          let _scroll = document.getElementById(scrollId);
          let _to = _scroll.offsetLeft + step;
          let _step_item;
          if (step > 0) {
            _to = _to >= 0 ? 0 : _to;
            _step_item = 12;
          } else {
            let _temp = -1 * (_scroll.clientWidth - this.scrollBoxWidth);
            _to = _to <= _temp ? _temp : _to;
            _step_item = -12;
          }
          let _endFunc = (f, t) => {
            return step < 0 ? f <= t : f >= t;
          };
          // console.log(_to, 'to');
          let _handle = setInterval(() => {
            let _step_to_ = _scroll.offsetLeft + _step_item;
            if (_endFunc(_step_to_, _to)) {
              _step_to_ = _to;
              this.scrolling=false;
              clearInterval(_handle);
              _scroll.style.left = _step_to_ + 'px';
              callback && callback();
              return;
            }
            _scroll.style.left = _step_to_ + 'px';
          }, 10);
        },
        scrollToLeft() {
          if(this.scrolling)return;
          this.doScroll(this.scrollElementId, (-1) * this.scrollStep, () => {
            //判断结束标志  //left<=-1*(内容宽度-滑动窗口宽度)
            // console.log(_scroll.style.left);
            let _scroll = document.getElementById(this.scrollElementId);
            let _currentLeft = _scroll.offsetLeft;
            // console.log('scrollToLeft')
            if (_currentLeft <= -1 * (_scroll.clientWidth - this.scrollBoxWidth)) {
              this.canScrollNext = false;
            } else {
              this.canScrollNext = true;
            }
            this.canScrollPrev = true;
          });
        },
        scrollToRight() {
          if(this.scrolling)return;
          this.doScroll(this.scrollElementId, this.scrollStep, () => {
            //判断结束标志   //left>=0
            // console.log(_scroll.offsetLeft);
            let _scroll = document.getElementById(this.scrollElementId);
            let _currentLeft = _scroll.offsetLeft;
            console.log('scrollToRight')
            if (_currentLeft >= 0) {
              this.canScrollPrev = false;
            } else {
              this.canScrollPrev = true;
            }
            this.canScrollNext = true;
          });
        }
      },
      components: {
        DeviceBox,
        NoDevice,
        DeviceItem
      }
    }
</script>

<style scoped>
  .devs-box{
    height: 472px;
    position: relative;
  }
  .devs-box:before{
    content: "";
    height: 1px;
    width: 100%;
    position: absolute;
    background: linear-gradient(to right, transparent, #40cef9, transparent);
    background: -ms-linear-gradient(to right,transparent, #40cef9, transparent);
    background: -webkit-linear-gradient(to right,transparent, #40cef9, transparent);
    background: -moz-linear-gradient(to right,transparent, #40cef9, transparent);
  }
  .devs-box:after{
    content: "";
    height: 1px;
    width: 100%;
    bottom: 0px;
    position: absolute;
    background: linear-gradient(to right, transparent, #40cef9, transparent);
    background: -ms-linear-gradient(to right,transparent, #40cef9, transparent);
    background: -webkit-linear-gradient(to right,transparent, #40cef9, transparent);
    background: -moz-linear-gradient(to right,transparent, #40cef9, transparent);
  }
  .devs-scroll-container{
    width: 1640px;
    height: 100%;
    overflow: hidden;
    position: relative;
    margin: 0 auto;
  }
  ul {
    position: absolute;
    display: block;
    white-space: nowrap;
    height: 472px;
    left: 0;
  }

  ul li {
    width: 410px;
    height: 420px;
    margin-top: 26px;
    display: inline-block;
    box-sizing: border-box;
    padding: 0 20px;
    /* border-left: 1px solid #000000; */
  }
  .scroll-button-next,.scroll-button-prev{
    position: absolute;
    top: 50%;
    transform: translateY(-50%);
    cursor: pointer;
  }
  .is-disabled{
    cursor: not-allowed;
  }
  /*div.scroll-button-next:disabled,div.scroll-button-prev:disabled{*/
  /*  cursor: auto;*/
  /*}*/
  .scroll-button-next img,.scroll-button-prev img{
    opacity: 0.4;
  }
  .scroll-button-next:hover img,.scroll-button-prev:hover img{
    opacity: 0.8;
  }
  /*div.scroll-button-next:disabled img,div.scroll-button-next:disabled img{*/
  .is-disabled:hover img,.is-disabled img{
    opacity: 0.2;
  }
  .scroll-button-prev{
    left: 0px;
  }
  .scroll-button-next{
    right: 0px;
  }
</style>

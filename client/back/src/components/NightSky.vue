<template>
  <div class="container" ref="box">
  </div>
</template>

<script>
    export default {
      name: "NightSky",
      methods:{
        starFactory() {
          if (this.$refs.box) {
            let width = this.$refs.box.offsetWidth - 20;
            let height = this.$refs.box.offsetHeight - 20;
            //随机生成星星
            for (let i = 0; i < 123; i++) {
              let img = document.createElement('img')
              // let cell=this.$refs.star;
              img.src = require("../assets/login/star.png")
              let lineHeight = Math.random() * 36 + 'px';
              img.className = 'night-sky-star';
              img.style.height = lineHeight;
              img.style.width = lineHeight;
              img.style.left = Math.random() * width + 'px'
              img.style.top = Math.random() * height + 'px'
              img.style.animationDelay = Math.random() * 3 + 's' //添加动画延迟时间
              this.$refs.box.appendChild(img);
            }
          }
        },
        lineFactory() {
          if (this.$refs.box) {
            let width = this.$refs.box.offsetWidth - 20;
            let height = this.$refs.box.offsetHeight - 20;
            //随机生成流星
            for (let i = 0; i < 7; i++) {
              let lx = document.createElement('div')
              lx.className = 'night-sky__star'
              lx.style.right = (Math.random() * width) + 'px'
              lx.style.top = (Math.random() * height) + 'px'
              lx.style.animationDelay = Math.random() * 3 * i + 's' //添加动画延迟时间
              this.$refs.box.appendChild(lx)
            }
          }
        }
      },
      mounted() {
        setTimeout(()=>{
          this.starFactory();
          this.lineFactory();
        },200);
      }
    }
</script>

<style scoped>
  .container {
    position: relative;
    width: 100vw;
    height: 100vh;
    /*background-color: #ff0;*/
    overflow: hidden;
  }
</style>

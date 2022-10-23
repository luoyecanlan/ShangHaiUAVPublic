<template>
  <div class="clock">
    {{ dateTime | formatDate }}
  </div>
</template>

<script>
import { formatTimeToStr } from "../modes/tool";

export default {
  name: "Clock",
  data() {
    return {
      dateTime: new Date()
    };
  },
  mounted() {
    setInterval(() => {
      this.dateTime = new Date();
    }, 500);
  },
  filters: {
    formatDate: time => {
      return formatTimeToStr(time);
    }
  }
};
</script>

<style scoped>
.clock {
  line-height: 60px;
  font-size: 24px;
  text-align: center;
  font-weight: bold;
  color: #999999;
}
</style>

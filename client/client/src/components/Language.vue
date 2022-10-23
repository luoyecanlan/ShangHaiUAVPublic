<template>
  <div class="langBox">
    <el-dropdown trigger="click" class="el-dd"  @command="handleCommand">
        <span class="el-dropdown-link">
          <img src="../assets/main/earth.png" style="margin-right: 5px;margin-top: 2px" alt="">
          <span style="line-height: 16px;">{{select.label}}</span>
        </span>
      <el-dropdown-menu slot="dropdown" >
        <el-dropdown-item v-for="(l,key) in langs" :key="key" :command="l">
          {{l.label}}
        </el-dropdown-item>
      </el-dropdown-menu>
    </el-dropdown>
  </div>
</template>

<script>
    export default {
        name: "Language",
        data() {
            return {
                langs: [
                    {
                        id: 0,
                        label: "中文",
                        lang: "zh-CN"
                    },
                    {
                        id: 1,
                        label: "English",
                        lang: "en-US"
                    },
                    /*{
                        id: 2,
                        label: "العربية",
                        lang: "ar-SA"
                    },*/
                ],
                select:{
                    id: 0,
                    label: "中文",
                    lang: "zh-CN"
                }
            };
        },
        methods: {
            handleCommand(command) {
                const {lang} = command;
                this.$i18n.locale = lang;
                this.select = command;
            }
        }
    };
</script>

<style scoped>

  .el-dropdown-link span{
    display: block;
    float: left;
  }
  .el-dropdown-link img{
    display: block;
    float: left;
  }
  .langBox{
    position: absolute;
    top: 5px;
    right: 100px;
    padding: 0;
    cursor: pointer;
    z-index:999;
  }
  .el-dd{
    width: 80px;
    color: #40cef9;
  }
</style>

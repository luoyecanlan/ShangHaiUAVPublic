<template>
  <div class="drawer-box__">
    <div class="list-box">
      <div class="list-title-box">
        <span class="bbox list-title bbox-bold"><img src="../../assets/table/title-arrow.png" alt=""> {{$t('m.addZone')}}</span>
      </div>
      <el-form
        class="list-container" style="margin-bottom:0px;padding: 45px 0px 20px"
        label-width="120px"
      >
        <div class="tb-border-lt"></div>
        <div class="tb-border-rt"></div>
        <div class="tb-border-rb" style="bottom: 46px"></div>
        <div class="tb-border-lb" style="bottom: 46px"></div>
        <div class="scroll-box">
          <div class="scroll" style="border-top: none">
            <div class="group-box__">
              <el-form-item :label="$t('m.zoneName')+'：'">
                <el-input maxlength="20" v-model="zoneName"size="mini" ></el-input>
              </el-form-item>
            </div>
          </div>
        </div>

        <div class="fixed-button-box__">
          <button class="option-button__ _ok__" @click.stop="doSave">
            {{$t('m.save')}}
          </button>
          <button class="option-button__ _cancel__" @click.stop="doReset">
            {{$t('m.reset')}}
          </button>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script>
  import {http_request,create_warn_zone} from "../../modes/api";
  import {msg_enum, show_message} from "../../modes/elementUI";

  export default {
    name: "Create",
    data(){
      return{
        zoneName:''
      }
    },
    methods:{
      doSave() {
        const {zoneName} = this;
        if(zoneName===""){
          show_message(msg_enum.error, this.$t('m.dataCannotBeNull'));
          return false;
        }
        if(zoneName.length>20){
          show_message(msg_enum.error,this.$t('m.lengthCannotBeyond')+'20');
          return false;
        }
        http_request(create_warn_zone,
          {name: zoneName},
          data => {
          this.$emit("success",data.id);
        });
      },
      doReset(){
        this.zoneName="";
      }
    }
  }
</script>

<style scoped src="../../assets/table/table.css"></style>

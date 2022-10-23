<template>
  <div class="list-box">
    <div class="list-title-box">
      <span class="bbox list-title" is-title><img src="../../assets/table/title-arrow.png" alt=""> 令牌列表</span>
<!--      <span class="bbox list-add"><img src="../../assets/table/option-add.png"> 添加项目</span>-->
      <span class="bbox list-search">
					<input type="text" v-model="key" @keyup.enter="initData" placeholder="点击搜索">
					<img src="../../assets/table/search.png" class="link-button" @click.stop="initData" alt="" height="16">
				</span>
    </div>
    <div class="list-container">
      <div class="tb-border-lt"><div></div></div>
      <div class="tb-border-rt"><div></div></div>
      <div class="tb-border-rb"><div></div></div>
      <div class="tb-border-lb"><div></div></div>
      <div class="list-header">
        <span class="list-header-item">用户名</span>
        <span class="list-header-item">用户角色</span>
        <span class="list-header-item">授权时间</span>
        <span class="list-header-item">生效时长（秒）</span>
        <span class="list-header-item">令牌状态</span>
        <span class="list-header-item">操作</span>
      </div>
      <div class="list-scroll-box">
        <div class="scroll-box">
          <div class="list-item" v-for="(item,key) in tableData" :key="key">
            <span class="list-item-cell">{{item.username}}</span>
            <span class="list-item-cell">{{ roleName(item) }}</span>
            <span class="list-item-cell">{{item.nbf}}</span>
            <span class="list-item-cell">{{item.expire_in}}</span>
            <span class="list-item-cell"><span :style="tokenStyle(item)">{{tokenStatus(item)}}</span></span>
            <span class="list-item-cell">
<!--              <img src="../../assets/table/view.png" class="link-button" @click.stop="doView(item)">-->
              <img src="../../assets/table/token.png" class="link-button" @click.stop="disabledToken(item.uid)">
							</span>
          </div>
        </div>
      </div>
    </div>
    <div class="list-paging-box">
      <div class="paging-box">
        <el-pagination background :pager-count="5"
                       :hide-on-single-page="false" class="page-layout"
                       :total="total" :page-size="size" :current-page.sync="index"
                       prev-text="上一页" next-text="下一页"
                       layout="prev, pager, next"
                       @current-change="initData"></el-pagination>
      </div>
    </div>
        <el-drawer :visible.sync="isView" @opened="openViewer" direction="rtl" model>
          <token-viewer ref="viewer" :info="select"/>
        </el-drawer>
  </div>
</template>

<script>
  import { http_request, request_tokens,distory_token,isTokenOK} from "../../modes/api";
  import { show_confirm,show_message, msg_enum } from "../../modes/elementUI";
  import {roleType} from "../../modes/tool";
  import TokenViewer from './Viewer'
  export default {
    name: "Index",
    data() {
      return {
        roleType,
        select:{},
        isView: false,
        tableData: [],
        index:1,
        key:"",
        size:10,
        total: 50
      };
    },
    methods: {
      roleName(info){
        return this.roleType.find(f=>f.value===info.rold).label;
      },
      tokenStatus(info){
        const {nbf,expire_in}=info;
        let isExpired= isTokenOK(nbf,expire_in);
        return isExpired?"已失效...":"生效中...";
      },
      doView(info){
        this.select=info;
        this.isView=true;
      },
      tokenStyle(info){
        const {nbf,expire_in}=info;
        let isExpired= isTokenOK(nbf,expire_in);
        return isExpired?{
          fontWeight:'bold',
          color:'#ffbc8a'
        }:{
          fontWeight:'bold',
          color:'#35cdff'
        };
      },
      openViewer(){
        //this.$refs.viewer.InitView();
      },
      onPageChange() {
        console.log("page info " + JSON.stringify(this.pagination));
        // show_message(msg_enum.info,page);
      },
      disabledToken(uid){
        show_confirm('提示','请确定是否强制失效令牌？',()=>{
          http_request(distory_token,uid,()=>{
            show_message(msg_enum.success,'操作成功！');
            this.initData();
          });
        });
      },
      initData() {
        const {index,key,size}=this;
        http_request(request_tokens, {index,key,size}, _data => {
          const {data, snumSize} = _data;
          this.tableData = data;
          this.total = snumSize;
        });
      }
    },
    mounted() {
      this.initData();
    },
    components:{TokenViewer}
  };
</script>

<style scoped src="../../assets/table/table.css">
/*.icon-jinyong{*/
/*  color: #ff4d51;*/
/*}*/
</style>

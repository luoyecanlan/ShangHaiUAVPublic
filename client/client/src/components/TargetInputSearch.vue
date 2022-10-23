<template>
  <div class="search-box__">
    <input class="search-input__"
           id="inputOpt"
           slot="reference"
           :placeholder="$t('m.enterIDSearchTarget')" type="text"
           v-model="searchText" @keyup.enter="querySearch"/>
    <i class="el-icon-search" @click.prevent="querySearch"></i>
    <transition name="fade">
      <div class="result-box__" v-show="searchResults.length">
        <div>
          <p class="result-item__" v-for="(tag,key) in searchResults"
             @click="selectHandle(tag)" :key="key">
            {{tag.id}}
          </p>
        </div>
        <i class="el-icon-close" @click.stop="closeResult"></i>
      </div>
    </transition>
  </div>
</template>

<script>
    import {mapActions, mapState} from "vuex";
    import {Fly_ToByTaergetID} from "../map/mapHandle/mapMove";
    import {featureType_enum, Select_TargetDraw} from "../map/mapHandle/mapDraw";
    import {Get_Target_Layer} from "../map/js";
    import HoverBox from '../components/OverBox'
    import {show_message, msg_enum} from "./../modes/elementUI";

    export default {
        name: "TargetInputSearch",
        data() {
            return {
                searchText: "",
                openSetting: false,
                searchResults: []
            }
        },
        methods: {
            ...mapActions(['set_select_target_id']),
            closeResult() {
                this.searchResults = [];
                this.searchText = "";
            },
            querySearch() {
                const {searchText, targets} = this;
                console.log(searchText);
                this.searchResults = searchText ? targets.filter(f => f.id.indexOf(searchText) >= 0) : targets;
            },
            selectHandle(_item)
            {
                let SelectFeature = Get_Target_Layer().getSource().getFeatureById(_item.id.toString());

                if (null == SelectFeature) {
                    show_message(msg_enum.error, this.$t('m.noTargetFound'));
                    return;
                }

                let ids = Select_TargetDraw(SelectFeature,featureType_enum.target);
                console.log('ids',ids);
                this.set_select_target_id({id:ids[0],did:ids[1]});
                Fly_ToByTaergetID(_item.id, featureType_enum.target);
                this.closeResult();
            },
        },
        components: {
            HoverBox
        },
        computed: {
            ...mapState(['targets'])
        }
    }
</script>

<style scoped>
  .search-box__ {
    position: absolute;
    height: 44px;
    width: 363px;
    top: 0px;
    right: 0px;
    padding: 4px 20px;
  }

  .result-box__ {
    position: absolute;
    background-color: #020e2c;
    width: 320px;
    max-width: 500px;
    color: #40CEF9;
    padding: 10px;
    top: 36px;
    z-index: 3000;
    overflow-y: auto;
    border: 1px solid #40CEF9;
    box-shadow: 0 0 5px 5px rgba(64, 204, 249, 0.4);
    -webkit-box-shadow: 0 0 5px 5px rgba(64, 204, 249, 0.4);
  }

  .search-input__ {
    width: 300px;
    line-height: 26px;
    outline: none;
    border: none;
    background-color: unset;
    font-size: 14px;
    color: #43DCFF;
  }

  input::-webkit-input-placeholder {
    color: rgba(64, 204, 249, 0.6);
  }

  .el-icon-close {
    position: absolute;
    top: 8px;
    right: 8px;
  }

  .el-icon-search, .el-icon-close {
    cursor: pointer;
    color: rgba(64, 204, 249, 0.6);
  }

  .el-icon-search:hover, .el-icon-close:hover {
    color: rgba(64, 204, 249, 1);
  }

  .result-item__ {
    font-family: "Microsoft YaHei";
    line-height: 26px;
    padding-left: 10px;
    cursor: pointer;
    border-bottom: 1px solid rgba(64, 204, 249, 0.05);
  }

  .result-item__:hover {
    color: #ffff00;
    background-color: rgba(64, 204, 249, 0.1);
  }
</style>

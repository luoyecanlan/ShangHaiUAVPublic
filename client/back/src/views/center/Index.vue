<template>

</template>

<script>
    import {show_message,msg_enum} from "../../modes/elementUI";
    import {http_request, update_current_pwd,request_current, update_current} from "../../modes/api";
    import {validator_phone, validator_required_rang, validator_type} from "../../modes/tool";
    import {type_enum} from "../../modes/tool/ruleType";

    export default {
        name: "Index",
        data()
        {
            return {
                UpDateUserInfoForm:
                    {
                    name: "",
                    nick: "",
                    phone: "",
                    email: "",
                    id: 0,
                },
                UpDateUserInfoRules:
                    {
                        name: validator_required_rang(1, 25, "name"),
                        nick: validator_required_rang(1, 25, "nick"),
                        phone: validator_phone(),
                        email: validator_type(type_enum.email)
                    }
            };
        },
        methods: {
            submitHandle()
            {
                this.$refs["UpDateUserInfoForm"].validate(valid =>
                {
                    if (valid)
                    {
                        http_request(update_current, this.UpDateUserInfoForm, () =>
                        {
                            show_message(msg_enum.success, "修改成功！");
                            this.$emit("success");
                        });
                    }
                });
            },
            gotoUpdatePassword()
            {

            }
        },
        mounted()
        {
            //获取当前用户信息
            http_request(request_current,'',(data)=>
        {
            this.UpDateUserInfoForm.name = data.name;
            this.UpDateUserInfoForm.email = data.email;
            this.UpDateUserInfoForm.phone = data.phone;
            this.UpDateUserInfoForm.nick = data.nick;
            this.UpDateUserInfoForm.id = data.uid;
        });
        }
    }
</script>

<style scoped>

</style>

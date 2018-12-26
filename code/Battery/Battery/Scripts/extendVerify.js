//自定义验证规则 (预设)
//required（必填项）
//phone（手机号）
//email（邮箱）
//url（网址）
//number（数字）
//date（日期）
//identity（身份证）
layui.use(['form'], function () {
    var form = layui.form;
    form.verify({
        //验证长度 需要加 属性【lay_vmin】【lay_vmax】
        length: function (value, item) {
            var min =$("#" + item.id).attr("lay-vmin")
            var max = $("#" + item.id).attr("lay-vmax")  
            if ((value.length > max || value.length < min) && value.length>0) {
                return '长度为' + min + "到" + max+"个字符";
            } 
        }
        //验证与另一个文本框是否一致【lay-vcontrast】对比容器的id
      ,equal: function (value, item) {
          var old = $("#" + item.id).attr("lay-vcontrast"); 
          var oldValue = $("#" + old).val(); 
          if (oldValue != value)
          {
              return '两次输入不一致';
          }
      }
        , requiredImg: function (value, item)
        {
            if (value.length<1)
            {
                return '请上传二维码';
            }
        }
        , IsUrl: function (value, item)
        { 
            if (value.length >0) {
                var regUrl = /http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/;
                var result = value.match(regUrl);
                if (result == null) {
                    return '请输入正确的URl';
                } 
            }
        }
        //根据正则表达式验证属性【regularexpression】既是表达式内容,例如^[-+]?\d*$  验证是否整数;【regularalert】表示要提示的信息
        /**
         * 通用正则集合
          NotNInt: /^\d+$/, //非负整数
          PlusInt: /^[0-9]*[1-9][0-9]*$/, //正整数
          NotPInt: /^((-\d+)|(0+))$/, //非正整数
          NegInt: /^-[0-9]*[1-9][0-9]*$/, //负整数
          Int: /^-?\d+$/, //整数
          NotNFloat: /^\d+(\.\d+)?$/, //非负浮点数
          PlusFloat: /^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$/, //正浮点数
          NotPFloat: /^((-\d+(\.\d+)?)|(0+(\.0+)?))$/, //非正浮点数
          NegFloat: /^(-(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)))$/, //负浮点数
          Float: /^(-?\d+)(\.\d+)?$/, //浮点数
          IsEmail: /^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$/, //邮箱
          IsIDCard: /^[1-9](\d{14}|\d{16}[\dxX])$/,//身份证号
          IsIP: /^(\d+)\.(\d+)\.(\d+)\.(\d+)$/,//IP地址
          IsPhone: /^(((\()?\d{2,4}(\))?[-(\s)*]){0,2})?(\d{7,8})$/, //固定电话号码
          IsMobilePhone: /^((\(\d{3}\))|(\d{3}\-))?(13\d{9}$)|(15\d{9}$)|(18\d{9}$)|(17\d{9}$)|(147\d{8}$)/, //移动电话
          IsAllTel: /^[0-9][0-9-#]*$/,
          IsZipCode: /^[1-9]\d{5}$/, //邮政编码
          IsDate: /((^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(10|12|0?[13578])([-\/\._])(3[01]|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(11|0?[469])([-\/\._])(30|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(0?2)([-\/\._])(2[0-8]|1[0-9]|0?[1-9])$)|(^([2468][048]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([3579][26]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][13579][26])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][13579][26])([-\/\._])(0?2)([-\/\._])(29)$))/,
          IsUrl: /http(s)?:\/\/([\w-]+\.)+[\w-]+(\/[\w- .\/?%&=]*)?/,
          IsChinese: /[\u4e00-\u9fa5]/, //中文
          IsLogin: /^[a-zA-Z0-9_\u4e00-\u9fa5]{1,}$/,//字母数字下环线和汉字
          IsLogin1: /^[a-zA-Z0-9_]{1,}$/,//字母下划线数字
          IsPassword: /^[a-zA-Z0-9_]{6,}$/,//字母下划线数字
          IsEnglish: /^[A-Za-z]+$/,//英文
          IsSpecial: /^[<>]/,
          IsNotSpecial: /^[^<>]*$/,
          IsMoney: /(^\d{1,18}$)|(^(\d{1,17}\.\d{1})$)|(^(\d{1,16}\.\d{2})$)/,
          IsPhone_1: /^[\d*#]{1,20}$/,
          IsValName: /^[0-9a-zA-Z_\u4e00-\u9fa5]*$/,
          IsFax: /^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$/,
          IsBankCard: /^\d{4}(?:\s\d{4}){3}$/
          */      
        , regularExpression: function (value, item) {
            var reg = "/" + $("#" + item.id).attr("regularexpression") + "/";
            console.log(reg);
            var aet = $("#" + item.id).attr("regularalert");
            var re = new RegExp(eval(reg));
            var r = value.match(re);
            if (r==null) {
                return aet; 
            }    
        }
    }); 
})

String.prototype.Trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "");
}
var chars = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];

function GetSjNo(n) {
    var res = "";
    for (var i = 0; i < n ; i++) {
        var id = Math.ceil(Math.random() * 35);
        res += chars[id];
    }
    return res;
} 
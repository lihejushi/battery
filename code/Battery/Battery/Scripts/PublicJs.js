var PubAjax = {};
; (function () {
    var _msg = PubAjax = (function () {
        var Option = {
            opacity: 0.2,
            fixed: true,
            lock: true
        };
        function ShowWaiting(_content) {
            VMsg.Waiting({ content: _content });
        }
        function CloseWaiting() {
            VMsg.Close("loading");
        }
        return {
            Get: function (url, data, callback, dataType, IsAsync, beforecallback, errorcallback) {
                if (typeof beforecallback != "function") beforecallback = function () { };
                if (typeof errorcallback != "function") errorcallback = function () { };
                if (typeof data == "object") data = $.extend({}, data, { "rad": Math.random() });
                ShowWaiting("正在获取数据....");
                $.ajax({
                    url: url,
                    type: "GET",
                    data: data,
                    dataType: dataType ? dataType : "json",
                    async: IsAsync == undefined ? true : IsAsync,
                    beforeSend: function (XMLHttpRequest) { beforecallback(); },
                    error: function () { CloseWaiting(); VMsg.AlertError( "系统繁忙，请稍后再试"); errorcallback(); },
                    success: function (d) {
                        CloseWaiting();
                        if ((dataType && dataType.toLowerCase != 'json') || CheckJsonResult(d)) {
                            if (callback != null) callback(d);
                        }
                    }
                });
            },
            Post: function (url, data, callback, dataType, IsAsync, beforecallback, errorcallback) {
                if (typeof beforecallback != "function") beforecallback = function () { };
                if (typeof errorcallback != "function") errorcallback = function () { };
                if (typeof data == "object") data = $.extend({}, data, { "rad": Math.random() });
                ShowWaiting("正在提交数据....");
                $.ajax({
                    url: url,
                    type: "POST",
                    data: data,
                    dataType: dataType ? dataType : "json",
                    async: IsAsync == undefined ? true : IsAsync,
                    beforeSend: function (XMLHttpRequest) { beforecallback(); },
                    error: function () { CloseWaiting(); VMsg.AlertError( "系统繁忙，请稍后再试"); errorcallback(); },
                    success: function (d) {
                        CloseWaiting();
                        if ((dataType && dataType.toLowerCase != 'json') || CheckJsonResult(d)) {
                            if (callback != null) callback(d);
                        }
                    }
                });
            },
            Delete: function (url, data, callback, dataType, IsAsync, beforecallback, errorcallback) {
                if (typeof beforecallback != "function") beforecallback = function () { };
                if (typeof errorcallback != "function") errorcallback = function () { };
                if (typeof data == "object") data = $.extend({}, data, { "rad": Math.random() });
                ShowWaiting("正在删除数据....");
                $.ajax({
                    url: url,
                    type: "DELETE",
                    data: data,
                    dataType: dataType ? dataType : "json",
                    async: IsAsync == undefined ? true : IsAsync,
                    beforeSend: function (XMLHttpRequest) { beforecallback(); },
                    error: function () { CloseWaiting(); VMsg.AlertError( "系统繁忙，请稍后再试"); errorcallback(); },
                    success: function (d) {
                        CloseWaiting();
                        if ((dataType && dataType.toLowerCase != 'json') || CheckJsonResult(d)) {
                            if (callback != null) callback(d);
                        }
                    }
                });
            }
        }
    })();
})();
(function ($) {
    if (window.VMsg == undefined) window.VMsg = {};
    //全局弹出
    $.extend(window.VMsg, {
        Option: {

        },
        //提示错误
        AlertError: function (msg, option) {//, time: 2000 
            var _option = $.extend({ icon: 2, title: "提示"}, this.Option, option);
            var index = layer.alert(msg, _option);
            return index;
        },
        //提示成功
        AlertSuccess: function (msg, option) {//, time: 2000
            var _option = $.extend({ icon: 1, title: "提示" }, this.Option, option);
            var index = layer.alert(msg, _option);
            return index;
        },
        //交互提示
        Confirm: function (msg, yes, option) {
            var _option = $.extend({ title: "提示!", icon: 3 }, this.Option, option);
            var index = layer.confirm(msg, { icon: 3, title: '提示' }, yes);
            return index;
        },
        //提示消息
        Waiting: function (option) {
            var index = layer.load();
            return index;
        },
        //弹出层
        Form: function (option) { 
            var _option = $.extend({
                type: 1,
            }, this.Option, option);
            var index = layer.open(_option);
            return index;
        },
        //弹出层引用页面
        Open: function (src, option) { 
            var _option = $.extend({}, this.Option, option, { content: src });
            var index = layer.open(_option);
            return index;
        },
        OpenWindow: function (url, title, option) {
            var _option = $.extend({
                type: 2,
                title: title,
                content: url,
                maxmin: false
            }, option);
            var index = layer.open(_option);
            return index;
        },
        TopOpenWindow: function (url, title, option) {
          
            if (top.layer) {
                var _option = $.extend({
                    type: 2,
                    title: title,
                    content: url,
                    maxmin: false
                }, option);
                var index = top.layer.open(_option);
                return index;
            } else {
                alert("Top层不包含layer");
            }
        },
        //弹出层ajax请求内容
        //Ajax: function (src, option) {
        //    var _option = $.extend({}, this.Option, option);
        //    var dialog = art.dialog(_option);
        //    $.ajax({
        //        url: src,
        //        success: function (data) {
        //            dialog.content(data);
        //        },
        //        cache: false
        //    });
        //    return dialog;
        //},
        Close: function (id) {
            if (id) {
                if (typeof (id) == "number") layer.close(id);
                else if (typeof (id) == "string") layer.closeAll(id)
            } else {
                layer.closeAll();
            }
        }
    });
})(jQuery);
/* 
功能：获取页面地址与参数
使用：var req = GetRequest();
req["href"]       获取页面地址
req[参数名称]   获取指定参数值，为空返回undefined
*/
function GetRequest(p) {
    var purl = window.location.href; if (p) purl = p;
    var theRequest = new Object();
    //获取地址
    var pstr = purl.split("/");
    theRequest["href"] = pstr[pstr.length - 1].match(/[^#?]+/i).toString();
    //存在参数
    if (purl.indexOf("?") != -1) {
        var url = purl.split("?")[1]; //获取url中"?"符后的字串（不包括?）
        var strs = url.split("&");
        for (var i = 0; i < strs.length; i++) {
            if (strs[i] != "") theRequest[strs[i].split("=")[0]] = decodeURIComponent(strs[i].split("=")[1]);
        }
    }
    return theRequest;
}
/************
倒计时提示
*************/
function CountDown(Counts, Tip, e) {
    var $LockID = $(e),
        ReTip = $LockID.text(),
        NewTip;
    var timer = setInterval(function () {
        if (Counts >= 0) {
            NewTip = Counts + Tip;
            $LockID.attr("disabled", "disabled");
            $LockID.text(NewTip);
            Counts = Counts - 1;
        } else {
            clearInterval(timer);
            $LockID.removeAttr("disabled");
            $LockID.text(ReTip);
        }
    }, 1000);
};

function CheckJsonResult(r) {
    if (r.code == "-98") {
        top.VMsg.AlertError("没有权限执行此操作");
        return false;
    } else {
        return true;
    }
}
function OpenWindow(url, title, option) {
    var _option = $.extend({
        type: 2,
        title: title,
        content: url,
      //  maxmin: false,
       // move: false
        area: ["750px", "610px"]
    }, option);
    if (!_option.end) _option.end = function () {
        location.href = location.href;
    };
    var index = VMsg.TopOpenWindow(url, title, _option);;
    //layer.full(index);

    //var index = layer.open(_option);
    //layer.full(index);
    return index;
}

function OpenFullWindow(url, title, option) {
    var _option = $.extend({
        type: 2,
        closeBtn: 0, //不显示关闭按钮
        title: title,
        content: url,
        maxmin: false,
        move: false
      
    }, option);
    if (!_option.end) _option.end = function () {
        location.href = location.href;
    };
    var index = layer.open(_option);
    layer.full(index);
    return index;
}

// 生成分页信息
// iCurrPage 当前页 iPageSize 页大小 iProCount 信息总条数 fun 回调函数 dive 显示分页信息容器
function initPageNav(iCurrPage, iPageSize, iProCount, fun, dive) {
    //this.pc = function (n) { fun.pageChange(n); };
    if (!iPageSize) iPageSize = 15;
    if (!iProCount) iProCount = 0;
    if (!iCurrPage) iCurrPage = 1;
    var b = ((iProCount % iPageSize) != 0);
    var iPageCount = parseInt(iProCount / iPageSize) + (b ? 1 : 0);
    if (iCurrPage > iPageCount) iCurrPage = iPageCount;
    iCurrPage = parseInt(iCurrPage);

    var sTemp3 = iProCount == 0 ? '' : '<div class="dataTables_info">从 ' + (((iCurrPage - 1) * iPageSize) + 1) + ' 到 ' + (iCurrPage * iPageSize > iProCount ? iProCount : iCurrPage * iPageSize) + '/共 ' + iProCount + ' 条数据</div>';
    var sTemp = iProCount == 0 ? '' : '<div class="row"><div class="col-xs-6">' + sTemp3 + '</div><div class="col-xs-6"><div class="dataTables_paginate paging_simple_numbers"><ul class="pagination">';
    var sTemp1 = iProCount == 0 ? '' : '';
    var sTemp2 = iProCount == 0 ? '' : '</ul></div></div></div>';

    if (iProCount != 0) {
        if (iPageCount == 1) sTemp1 = '<li class="paginate_button previous disabled"><a href="javascript:void(0);">前一页</a></li><li class="paginate_button active"><a href="javascript:void(0);">' + iCurrPage + '</a></li><li class="paginate_button next disabled"><a href="javascript:void(0);">下一页</a></li>';
        else if (iPageCount == iCurrPage)
            sTemp1 = '<li class="paginate_button previous"><a href="javascript:void(0);"  targetIndex="' + (iCurrPage - 1) + '">前一页</a></li><li class="paginate_button active"><a href="javascript:void(0);">' + iCurrPage + '</a></li><li class="paginate_button next disabled"><a href="javascript:void(0);">下一页</a></li>';
        else if (iCurrPage == 1)
            sTemp1 = '<li class="paginate_button previous disabled"><a href="javascript:void(0);">前一页</a></li><li class="active"><li class="paginate_button active"><a href="javascript:void(0);">' + iCurrPage + '</a></li><li class="paginate_button next"><a href="javascript:void(0);" targetIndex="' + (iCurrPage + 1) + '">下一页</a></li>';
        else
            sTemp1 = '<li class="paginate_button previous"><a href="javascript:void(0);"  targetIndex="' + (iCurrPage - 1) + '">前一页</a></li><li class="paginate_button active"><a href="javascript:void(0);">' + iCurrPage + '</a></li><li class="paginate_button next"><a href="javascript:void(0);"  targetIndex="' + (iCurrPage + 1) + '">下一页</a></li>';
    }

    var html = sTemp + "  " + sTemp1 + "  " + sTemp2;
    $(dive).html(html).find("a[targetIndex]").click(function () {
        var _p = $(this).attr("targetIndex");
        fun.pageChange(parseInt(_p));
    });
}
var DataPage = function (iCurrPage, iPageSize, iProCount) {
    this.CurrPage = iCurrPage;
    this.PageSize = iPageSize;
    this.ProCount = iProCount;
    this.init = function () {
        initPageNav(this.CurrPage, this.PageSize, this.ProCount, this, ".pages");
    };
    this.pageChange = function (n) {
        //this.CurrPage = n; this.init();
        var req = GetRequest();
        location.href = ChangeQueryString("page", n, req["href"]);
    };
    this.init();
}
 
//上传图片
var UpImgSingle_H = false;
function checkEx(filename) {
    var exts = ["jpg", "jpeg", "png"];
    var AllowExt = false;
    for (var i = 0; i < exts.length; i++) {
        var myregex = new RegExp('.*\.' + exts[i] + '$');
        if (myregex.test(filename.toLowerCase())) return true;
    }
    return AllowExt;
}
function UploadImg(e, CallBackImg) {
    if (UpImgSingle_H == false) {
        //判断后缀
        var extension = $(e).val();
        if (checkEx(extension) == false) {
            VMsg.AlertError("图片格式不符合规定！");
            var tempForm = document.createElement('form');
            $(e).before(tempForm);
            $(tempForm).append(e);
            tempForm.reset();
            $(tempForm).after(e);
            $(tempForm).remove();
            return false;
        }
        if (e.files[0].size > 3 * 1024 * 1024) {
            VMsg.AlertError("图片大小超过3M！");
            return false;
        }
        UpImgSingle_H = true;
        var fileid = $(e).attr("id"); 
        $.ajaxFileUpload({ 
            url: UrlPath + "Common/UpImage",
            fileElementId: fileid, secureuri: false, dataType: 'json',
            success: function (json, status, data) { 
                $(":file").val(""); 
                if (json.code == "1") {
                    CallBackImg(json.data);
                } else
                    VMsg.AlertError(json.Message);
                UpImgSingle_H = false;
            },
            error: function (data, status, sender) {  
                UpImgSingle_H = false;
                $(":file").val("");  
            }
        });
    }
    return false;
}
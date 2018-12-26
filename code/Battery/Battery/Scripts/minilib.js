var PubAjax = {};
; (function () {
    var _msg = PubAjax = (function () {

        var LayerIndex = 0;
        function ShowWaiting(_content) {
            $.toast(_content, 20000);
            //LayerIndex = layer.open({ type: 2, content: _content });
        }
        function CloseWaiting() {
            $.closeModal();
        }
        return {
            Get: function (url, data, callback, dataType, IsAsync, beforecallback, errorcallback) {
                if (typeof beforecallback != "function") beforecallback = function () { };
                if (typeof errorcallback != "function") errorcallback = function () { };
                data = $.extend({}, data, { "rad": Math.random() });
                ShowWaiting("正在获取数据....");
                $.ajax({
                    url: url,
                    type: "GET",
                    data: data,
                    dataType: dataType ? dataType : "json",
                    async: IsAsync == undefined ? true : IsAsync,
                    beforeSend: function (XMLHttpRequest) { beforecallback(); },
                    error: function () { CloseWaiting(); PageAlert.XTAlert({ content: "系统繁忙，请稍后再试" }); errorcallback(); },
                    success: function (d) {
                        CloseWaiting();
                        if ((dataType && dataType.toLowerCase != 'json') || CheckJsonResult(d)) {
                            if (callback != null) callback(d);
                        }
                    }
                });
            },
            GetScript: function (url, data, callback, dataType, IsAsync, beforecallback, errorcallback) {
                if (typeof beforecallback != "function") beforecallback = function () { };
                if (typeof errorcallback != "function") errorcallback = function () { };
                data = $.extend({}, data, { "rad": Math.random() });
                ShowWaiting("正在获取数据....");
                $.ajax({
                    url: url,
                    type: "GET",
                    data: data,
                    dataType: dataType ? dataType : "script",
                    async: IsAsync == undefined ? true : IsAsync,
                    beforeSend: function (XMLHttpRequest) { beforecallback(); },
                    error: function () { CloseWaiting(); PageAlert.XTAlert({ content:  "系统繁忙，请稍后再试" }); errorcallback(); },
                    success: function (d) {
                        CloseWaiting();
                        if (callback != null) callback(d);
                        //if ((dataType/* && dataType.toLowerCase != 'json'*/) || CheckJsonResult(d)) {
                            
                        //}
                    }
                });
            },
            Post: function (url, data, callback, dataType, IsAsync, beforecallback, errorcallback) {
                if (typeof beforecallback != "function") beforecallback = function () { };
                if (typeof errorcallback != "function") errorcallback = function () { };
                data = $.extend({}, data, { "rad": Math.random() });
                ShowWaiting("正在提交数据....");
                $.ajax({
                    url: url,
                    type: "POST",
                    data: data,
                    dataType: dataType ? dataType : "json",
                    async: IsAsync == undefined ? true : IsAsync,
                    beforeSend: function (XMLHttpRequest) { beforecallback(); },
                    error: function () { CloseWaiting(); PageAlert.XTAlert({ content:  "系统繁忙，请稍后再试" }); errorcallback(); },
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
                data = $.extend({}, data, { "rad": Math.random() });
                ShowWaiting("正在删除数据....");
                $.ajax({
                    url: url,
                    type: "DELETE",
                    data: data,
                    dataType: dataType ? dataType : "json",
                    async: IsAsync == undefined ? true : IsAsync,
                    beforeSend: function (XMLHttpRequest) { beforecallback(); },
                    error: function () { CloseWaiting(); PageAlert.XTAlert({ content:  "系统繁忙，请稍后再试" }); errorcallback(); },
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
//获取列表信息
function CheckJsonResult(r) {
    if (r && r.code == "-98") {
        PageAlert.XTAlert(r.message || "无法操作");
        return false;
    } else {
        return true;
    }
}
; (function ($) {
    if (window.PageAlert == undefined) window.PageAlert = {};
    //全局弹出
    $.extend(window.PageAlert, {
        Option: { content: "内容！" },
        SmallAlert: function (content)
        {
            $.alert(content,"提示");
            $('.weui-dialog__bd').css({ 'font-size': '14px' });
           // $.toast(content, "text");
        },
        XTAlert: function (content,title, ok) { 
            var index = $.alert(content, title, ok);
            $('.weui-dialog__bd').css({ 'font-size': '14px' }); 
            return index;
        },
        XTAlertConfirm: function (content, title, ok,cancel) { 
            var index = $.confirm({
                title: title,
                text: content,
                onOK:ok,
                onCancel: cancel
            });
            return index;
        },
        Close: function () {
            $.closeModal();
        }
    });
})(jQuery);
function GetNavjsonString(G) {
    if (G) {
        if (typeof (G) == "string") G = JSON.parse(G);

        var s = '<ol class="breadcrumb" style="margin-bottom:0px;margin-top:5px;">'
        s += '<li>' + G.NavContentTypeTitle + '</li>';
        if (G.NavContentType == "ZDY")
            s += '<li>' + G.NavContentValue + '</li>';
        else {
            if (G.NavContentType == "SHOP")
                s += '<li>' + G.NavContentValueTitle + '</li>';
            s += '<li>' + G.NavLinkTypeTitle + '</li>';
            s += '<li>' + G.NavLinkTitle + '</li>';
            if (G.NavLinkType == "Content")
                s += '<li>' + G.NavValueTitle + '</li>';
        }
        s += '</ol>';
        return s;
    }
    return "";
}

// 以下提供复选框的相关操作
/*******************************************
功能: 全选
参数: e 操作全选复选框，一般使用this
name 目标选择框的name
*******************************************/
function checkSelectAll(e, name) {
    if ($(e).prop('checked') == undefined || $(e).prop('checked') == false) $("input[name='" + name + "']").prop("checked", false);
    else $("input[name='" + name + "']").prop("checked", $(e).prop("checked"));
}

/*******************************************
功能: 获取所有复选框值
参数: name 目标选择框的name
返回: 使用,号分隔的一组值，如果没有任何选择项，返回""
*******************************************/
function checkValues(name) {
    var returnval = "";
    $("input[name='" + name + "']").each(function () {
        if ($(this).prop("checked")) {
            if (returnval == "") returnval = $(this).val();
            else returnval += "," + $(this).val();
        }
    });
    return returnval;
}
function checkValues_(name, falg, falg_value) {
    var returnval = "";
    $("input[" + falg + "='" + name + "']").each(function () {
        if (returnval == "") returnval = $(this).attr(falg_value);
        else returnval += "," + $(this).attr(falg_value);
    });
    return returnval;
}
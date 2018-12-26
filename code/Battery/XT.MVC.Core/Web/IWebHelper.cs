using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace XT.MVC.Core.Web
{
    public partial interface IWebHelper
    {
        /// <summary>
        /// 获取反向链接
        /// </summary>
        /// <returns>反向链接</returns>
        string GetUrlReferrer();

        /// <summary>
        /// 获取当前IP地址
        /// </summary>
        /// <returns>IP地址</returns>
        string GetCurrentIpAddress();

        /// <summary>
        /// 获取当前页面地址
        /// </summary>
        /// <param name="includeQueryString">是否包含查询</param>
        /// <returns>当前页面地址</returns>
        string GetThisPageUrl(bool includeQueryString);

        /// <summary>
        /// 获取当前页面地址
        /// </summary>
        /// <param name="includeQueryString">是否包含查询</param>
        /// <param name="useSsl">是否使用ssl</param>
        /// <returns>当前页面地址</returns>
        string GetThisPageUrl(bool includeQueryString, bool useSsl);

        /// <summary>
        /// 当前连接是否是安全连接
        /// </summary>
        /// <returns></returns>
        bool IsCurrentConnectionSecured();

        /// <summary>
        /// 根据名称获取服务器变量
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <returns>服务器变量</returns>
        string ServerVariables(string name);

        /// <summary>
        /// 获取服务器地址
        /// </summary>
        /// <param name="useSsl">是否使用SSL</param>
        /// <returns></returns>
        string GetAppHost(bool useSsl);

        /// <summary>
        /// 获取站点地址
        /// </summary>
        /// <returns></returns>
        string GetAppLocation();

        /// <summary>
        /// 获取站点地址
        /// </summary>
        /// <param name="useSsl">是否使用SSL</param>
        /// <returns></returns>
        string GetAppLocation(bool useSsl);

        /// <summary>
        /// 返回true,如果请求的资源是一个静态资源,则不必处理。
        /// </summary>
        /// <param name="request">HTTP Request</param>
        /// <returns>返回true 为静态资源</returns>
        /// <remarks>
        /// 静态资源包含后缀:
        /// .css
        ///	.gif
        /// .png 
        /// .jpg
        /// .jpeg
        /// .js
        /// .axd
        /// .ashx
        /// </remarks>
        bool IsStaticResource(HttpRequest request);

        /// <summary>
        /// 虚拟路径映射到物理路径
        /// </summary>
        /// <param name="path">虚拟地址，例如："~/bin"</param>
        /// <returns>物理路径，例如："c:\inetpub\wwwroot\bin"</returns>
        string MapPath(string path);


        /// <summary>
        /// 修改参数
        /// </summary>
        /// <param name="url">需要修改的URL</param>
        /// <param name="queryStringModification">修改内容</param>
        /// <param name="anchor">锚点</param>
        /// <returns></returns>
        string ModifyQueryString(string url, string queryStringModification, string anchor);

        /// <summary>
        /// 移除参数
        /// </summary>
        /// <param name="url">需要修改的URL</param>
        /// <param name="queryString">移除内容</param>
        /// <returns>New url</returns>
        string RemoveQueryString(string url, string queryString);

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">参数名称</param>
        /// <returns>参数值</returns>
        T QueryString<T>(string name);

        /// <summary>
        /// 重启站点
        /// </summary>
        /// <param name="makeRedirect">是否跳转 </param>
        /// <param name="redirectUrl">跳转地址</param>
        void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "");

        /// <summary>
        /// 判断请求是否来自搜索引擎
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns></returns>
        bool IsSearchEngine(HttpContextBase context);

        /// <summary>
        /// 指示是否客户端将被重定向到一个新的位置
        /// </summary>
        bool IsRequestBeingRedirected { get; }

        /// <summary>
        /// 指示是否客户端将被重定向到一个新的位置使用POST
        /// </summary>
        bool IsPostBeingDone { get; set; }

        /// <summary>
        /// 检查是否有非法输入
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        bool Filter(HttpRequest request, HttpResponse response);
    }
}

using TuoKe.DAL.WX;
using TuoKe.Framework.Domain;
using TuoKe.Framework.Tools;
using TuoKe.Model.WX;
using TuoKe.Service;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using XT.MVC.Core;
using XT.MVC.Core.Common;
using XT.MVC.Core.Encrypt;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Logging;
using XT.MVC.Core.Web;
using XT.MVC.Framework.Results;

namespace TuoKe.Framework.Filters
{
    /// <summary>
    /// 预先处理数据
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class WeixinOAuth2Attribute : ActionFilterAttribute
    {
        protected OAuthScope _scope = OAuthScope.snsapi_base;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope">OAuthScope.snsapi_base[基本信息], OAuthScope.snsapi_userinfo[全部信息]</param>
        public WeixinOAuth2Attribute(OAuthScope scope = OAuthScope.snsapi_base)
        {
            _scope = scope;
            this.Order = 10;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (filterContext.IsChildAction == false)
            {
                string openId = string.Empty, access_token = string.Empty;
                IWebHelper webHelper = EngineContext.Current.Resolve<IWebHelper>();
                string redirectUrl = filterContext.HttpContext.Request.Url.AbsoluteUri.ToString();
                redirectUrl = Regex.Replace(redirectUrl, @"(:(\d)+)", "");

                var request = filterContext.RequestContext.HttpContext.Request;
                string code = request.QueryString["code"];
                string state = request.QueryString["state"];  
          
                if (string.IsNullOrEmpty(code) == false && string.IsNullOrEmpty(state) == false)
                {
                    #region 如果包含有code和state时，默认为微信授权返回页 
                    if (state != WxConfig.OAout2_State)
                    {
                        filterContext.Result = new ContentResult()
                        {
                            Content = "验证失败！请从正规途径进入！"
                        };
                    }
                    else
                    {
                        OAuthAccessTokenResult result = null; 
                        try
                        {
                            result = OAuthApi.GetAccessToken(WxConfig.AppId, WxConfig.SecretKey, code);
                        }
                        catch (Exception ex)
                        {
                            EngineContext.Current.Resolve<ILogger>("DefaultLogger").Error(ex, "GetAccessToken：" + ex.Message);
                        }

                        #region
                        if (result != null)
                        {
                            filterContext.Controller.ViewData[ConstVar.WX_ViewBag_OAuth2] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            access_token = result.access_token;
                            openId = result.openid;
                            if (result.errcode != ReturnCode.请求成功)
                            {
                                filterContext.Result = new ContentResult()
                                {
                                    Content = "错误：" + result.errmsg
                                };
                            }
                            else
                            {
                                //更新数据
                               WxHelper.UpdateUserAccessToken(result.openid, result.access_token, result.expires_in, result.refresh_token);
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 跳转到授权页面

                    if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                        throw new InvalidOperationException("You cannot use [AppAuthorize] attribute when a child action cache is active");
               

                    #endregion
                }

                if (filterContext.HttpContext.Request.QueryString["TestOpenId"] != null)
                {
                    openId = filterContext.HttpContext.Request.QueryString["TestOpenId"];
                }

                if (string.IsNullOrEmpty(openId) == false)
                {
                    #region 更新用户信息
                    WX_SubscribeUsers user = UserDAL.GetUserModel(openId);
                    var info = user != null ? user : new WX_SubscribeUsers();
                    if (CheckAuthorize(filterContext) == false || string.IsNullOrEmpty(openId) == false)
                    {
                        #region 初次访问获取

                        try
                        {
                            var userInfo = UserApi.Info(WxConfig.AppId, openId);
                            info.Id = 0;
                            info.OpenId = openId;
                            info.IsUnSubscribe = 1;

                            int userId = 0;
                            if (userInfo.errcode == ReturnCode.请求成功 && userInfo.subscribe == 1)
                            {
                                info.City = userInfo.city;
                                info.Country = userInfo.country;
                                info.HeadImgUrl = userInfo.headimgurl;
                                info.NickName = userInfo.nickname;
                                info.Privilege = "";
                                info.Province = userInfo.province;
                                info.Sex = userInfo.sex;

                                if (UserDAL.UpdateUserInfo(info, out userId) > 0)
                                {
                                    UserCacheService.Instance.RemoveUserCache(info.OpenId);
                                    info.Id = userId;
                                    info.IsUnSubscribe = 0;

                                    //TuoKe.Framework.Tools.WxHelper.MemberUpdateByCityName(info.OpenId, info.City);
                                }
                            }
                        }
                        catch { }

                        #endregion
                    }
                    else
                    {
                        #region 再次访问
                        if (_scope == OAuthScope.snsapi_userinfo && string.IsNullOrEmpty(access_token) == false)//如果是要求信息则用户信息
                        {
                            try
                            {
                                var userInfo = OAuthApi.GetUserInfo(access_token, openId);
                                info.OpenId = openId;
                                info.City = userInfo.city;
                                info.Country = userInfo.country;
                                info.HeadImgUrl = userInfo.headimgurl;
                                info.NickName = userInfo.nickname;
                                info.Privilege = "";
                                info.Province = userInfo.province;
                                info.Sex = userInfo.sex;
                            }
                            catch { }
                        }
                        else
                        {
                            if (user != null && user.IsUnSubscribe == 0)//关注过才能获取数据
                            {
                                try
                                {
                                    var userInfo = UserApi.Info(WxConfig.AppId, user.OpenId);
                                    if (userInfo.errcode == ReturnCode.请求成功 && userInfo.subscribe == 1)
                                    {
                                        info.Id = user.Id;
                                        info.OpenId = user.OpenId;
                                        info.City = userInfo.city;
                                        info.Country = userInfo.country;
                                        info.HeadImgUrl = userInfo.headimgurl;
                                        info.NickName = userInfo.nickname;
                                        info.Privilege = "";
                                        info.Province = userInfo.province;
                                        info.Sex = userInfo.sex;
                                    }
                                }
                                catch { }
                            }
                        }
                        if (user != null)
                        {
                            UserDAL.UpdateSubscribeUser(info);
                            filterContext.Controller.ViewData[ConstVar.WX_ViewBag_UserInfo] = info;
                        }
                        #endregion
                    }

                    //用户登录
                    BaseLogin(
                        filterContext.RequestContext.HttpContext.Response,
                        UserType.App,
                        0,
                        user == null ? 0 : info.Id,
                        user == null ? string.Empty : info.NickName ?? string.Empty,
                        user == null ? string.Empty : info.NickName ?? string.Empty,
                        openId);
                    workContext.Refresh(null);
                    appContext.Refresh(null);

                    filterContext.Controller.ViewData[ConstVar.WX_ViewBag_UserInfo] = info;
                    if (string.IsNullOrEmpty(code) == false && string.IsNullOrEmpty(state) == false)
                    {
                        redirectUrl = webHelper.RemoveQueryString(redirectUrl, "state");
                        redirectUrl = webHelper.RemoveQueryString(redirectUrl, "code");
                        filterContext.Result = new RedirectResult(redirectUrl);
                    }

                    #endregion
                }
                else
                {
                    redirectUrl = webHelper.RemoveQueryString(redirectUrl, "state");
                    redirectUrl = webHelper.RemoveQueryString(redirectUrl, "code");
                    string oauthUrl = OAuthApi.GetAuthorizeUrl(
                                    WxConfig.AppId,
                                    redirectUrl,
                                    WxConfig.OAout2_State,
                                    _scope
                            );
                    if (string.IsNullOrEmpty(filterContext.RequestContext.HttpContext.Request.Headers["X-PJAX"]) == false)
                    {
                        filterContext.Result = new ContentResult() { Content = "<title>跳转</title><script>location.href='" + oauthUrl + "'</script>", ContentEncoding = Encoding.UTF8 };
                    }
                    else
                    {
                        if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                        {
                            XT.MVC.Framework.Result result = new XT.MVC.Framework.Result() { Code = -98, Message = "没有访问此接口的权限", Data = string.Empty };
                            filterContext.Result = new XTJsonResult(result, "yyyy-MM-dd HH:mm:ss");
                        }
                        else if (filterContext.HttpContext.Request.HttpMethod == "POST")
                        {
                            filterContext.Result = new ContentResult() { Content = "没有访问此接口的权限", ContentEncoding = Encoding.UTF8 };
                        }
                        else
                        {
                            //跳转到授权页面
                            filterContext.Result = new RedirectResult(oauthUrl);
                        }
                    }
                }
            }
            #endregion
            base.OnActionExecuting(filterContext);
        }
        /// <summary>
        /// 获取授权连接
        /// </summary>
        /// <param name="appId">公众号的appid</param>
        /// <param name="redirectUrl">重定向地址，需要urlencode，这里填写的应是服务开发方的回调地址</param>
        /// <param name="scope">授权作用域，拥有多个作用域用逗号（,）分隔</param>
        /// <param name="state">重定向后会带上state参数，开发者可以填写任意参数值，最多128字节</param>
        /// <param name="component_appid">服务方的appid，在申请创建公众号服务成功后，可在公众号服务详情页找到</param>
        /// <param name="responseType">默认为填code</param>
        /// <returns>URL</returns>
        public static string GetAuthorizeUrl(string appId, string redirectUrl, OAuthScope scope, string state, string component_appid, string responseType = "code")
        {
            var url =
                string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type={2}&scope={3}&state={4}&component_appid={5}#wechat_redirect",
                                appId, redirectUrl.UrlEncode(), responseType, scope, state, component_appid);
            return url;
        }
    }
}

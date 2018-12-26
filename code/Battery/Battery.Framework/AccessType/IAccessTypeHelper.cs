using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Battery.Framework.AccessType
{
    public interface IAccessTypeHelper
    {
        /// <summary>
        /// 是否移动设备访问
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        bool IsMobile();

        /// <summary>
        /// 是否是app访问
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        bool IsApp();

        /// <summary>
        /// 是否是微信访问
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        bool IsWeiXin();

        /// <summary>
        /// 是否是分享
        /// </summary>
        /// <returns></returns>
        bool IsShare();

        bool IsAndroid();
        bool IsIOS();
    }
}

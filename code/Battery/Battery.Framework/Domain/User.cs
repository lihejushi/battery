using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Domain.Users;

namespace Battery.Framework.Domain
{
    public enum UserType
    {
        Sys,//系统管理员账号
        Shop,//店铺管理账号
        App,//app用户账号
        Company
    }
    /// <summary>
    /// App用户,使用OpenId做主键
    /// </summary>
    public class User : IUser
    { 
        /// <summary>
        /// 默认是0,只有店铺管理有值
        /// </summary>
        public int ShopId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType UserType { get; set; }

        private bool _isLogin = false;

        public bool IsLogin
        {
            get { return (UserType == Domain.UserType.App && string.IsNullOrEmpty(OpenId)) ? false : _isLogin; }
            set { _isLogin = value; }
        }
        
        public string Avator { get; set; }

        private string _OpenId = string.Empty;
        /// <summary>
        /// 微信用户才有
        /// </summary>
        public string OpenId {
            get
            {
                if (UserType == Domain.UserType.App)
                {
                    return _OpenId;
                }
                return string.Empty;
            }
            set
            {
                if (UserType == Domain.UserType.App)
                {
                    _OpenId = value;
                }
                else
                {
                    _OpenId = string.Empty;
                }
            }
        }
    }
}

using TuoKe.DAL.WX;
using TuoKe.Model.WX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TuoKe.Framework.CurrentContext
{
    public interface IWxUser
    {
        WX_SubscribeUsers User { get; }

        void Refresh(string openId);

        void Refresh(int userId);
    }

    public class WxUser : IWxUser
    {
        WX_SubscribeUsers _User = null;

        public WX_SubscribeUsers User
        {
            get { return _User; }
        }

        public void Refresh(string openId)
        {
            if (_User == null || _User.OpenId != openId)
            {
                _User = UserDAL.GetUserModel(openId);
            }
        }

        public void Refresh(int userId)
        {
            if (_User == null || _User.Id != userId)
            {
                _User = UserDAL.GetUserModel(userId);
            }
        }
    }
}

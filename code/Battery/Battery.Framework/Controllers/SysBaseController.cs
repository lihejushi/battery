using Battery.DAL.Log;
using Battery.Model.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XT.MVC.Core.Common;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Web;
using XT.MVC.Framework.Controllers;

namespace Battery.Framework.Controllers
{ 
       public class SysBaseController : ContextController
    {
        IWebHelper webHelper = EngineContext.Current.Resolve<IWebHelper>();
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="actionType">
        /// 动作类型
        /// 0 浏览
        /// 1 新增
        /// 2 编辑
        /// 3 删除
        /// 4 变更状态
        /// 5 审核
        /// </param>
        /// <param name="memo">备注，请尽量填写详细</param>
        protected void InsertLog(ActionType actionType, string memo)
        {
            if (CurrentUser.IsLogin)
            {
                try
                {
                    SysLogDAL.AddPersonLog(new Model.Log.Person_Log()
                    {
                        ActionType = actionType,
                        LogIP = webHelper.GetCurrentIpAddress(),
                        LogTime = DateTime.Now,
                        Memo = memo,
                        PersonId = CurrentUser.UserId,
                        Url = Request.RawUrl.ToString()
                    });
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "后台用户操作记录：用户【{0}】，控制器：【{1}】，动作：【{2}】,ActionType：【{3}】,Memo：【{4}】", actionType, memo);
                }
            }
        }

        /// <summary>
        /// 写入用户日志
        /// </summary>
        /// <param name="UserActionType">
        /// 动作类型
        /// 0 浏览
        /// 1 新增
        /// 2 编辑
        /// 3 删除
        /// 4 变更状态
        /// 5 审核
        /// </param>
        /// <param name="memo">备注，请尽量填写详细</param>
        protected void InsertUserLog(UserActionType actionType, string memo,int result)
        {
            if (CurrentUser.IsLogin)
            {
                try
                {
                    SysLogDAL.AddUserLog(new Model.Log.User_Log()
                    {
                        OperatCode = actionType,
                        RegID = CurrentUser.UserId,
                        OperateIP = webHelper.GetCurrentIpAddress(),
                        OperateTime = DateTime.Now,
                        OPerate = memo,
                        OperateResult= result
                    });
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "后台用户操作记录：用户【{0}】，控制器：【{1}】，动作：【{2}】,ActionType：【{3}】,Memo：【{4}】", actionType, memo);
                }
            }
        }

        /// <summary>
        /// 检查是否有非法输入
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool Filter(System.Web.HttpRequest request, System.Web.HttpResponse response)
        {
            return webHelper.Filter(request,response);
        }

    }
}

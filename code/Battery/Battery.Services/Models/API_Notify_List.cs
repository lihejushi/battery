using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TuoKe.Services.Models
{

    public class API_Notify_List
    {
        public class NotifyMod
        {
            public NotifyMod()
            {
                this.No = "";
                this.Result = new ApiResult()
                {
                    Code = -1,
                    ResultMsg = "操作失败"
                };
            }
            public string No { get; set; }
            public ApiResult Result { get; set; }
            public bizContent bizContent { get; set; }
            public string tkContent { get; set; }
        }

        public class ApiResult
        {
            public ApiResult()
            {
                this.ResultMsg = "操作失败";
                this.Code = -1;
            }
            public int Code { get; set; }// 0验证成功  -1数据格式有误  -2资源无效 -3兑换码无效
            public string ResultMsg { get; set; }
        }
        public class CheckModForList
        {
            public string No { get; set; }
            public string Batch { get; set; }
            public string Count { get; set; }
            public List<ResourcesListTemp> ResourcesList { get; set; }

        }

        public class NotifyModForList
        {
            public NotifyModForList()
            {
                this.No = "";
                this.Result = new ApiResult()
                {
                    Code = -1,
                    ResultMsg = "操作失败"
                };
            }
            public string No { get; set; }
            public ApiResult Result { get; set; }
            public string Batch { get; set; }
            public string Count { get; set; }
            public List<ResourcesList> ResourcesList { get; set; }

        }

        public class ResourcesListTemp
        {
            public string bizContent { get; set; }

            public string tkContent { get; set; }
        }

        public class ResourcesList
        {
            public bizContent bizContent { get; set; }
            public string GroupActiveNo { get; set; } //加这个属性主要是后期分组方便
            public string GroupVerifyNo { get; set; } //加这个属性主要是后期分组方便,当KEY用，因为兑换码是绝对不会重复的
            public tkContent tkContent { get; set; }
        }

        public class tkContent
        {
            public string ActiveNo { get; set; }
            public string VerifyNo { get; set; }
        }



        public class bizContent
        {
            public string MobileNo { get; set; }
            public int NodeCode { get; set; }
            public string NodeMsg { get; set; }
            public string ProductName { get; set; }
            public string ProductDesc { get; set; }
            public string NodeTime { get; set; }

        }

        public class T_Company
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }
            public string CompanyIntro { get; set; }
            public string LoginName { get; set; }
            public string LoginPwd { get; set; }
            public string Address { get; set; }
            public string OfficePhone { get; set; }
            public string ContactName { get; set; }
            public string ContactEmail { get; set; }
            public string ContactMobile { get; set; }
            public string OrganizationCode { get; set; }
            public string LicenseNum { get; set; }
            public string Representative { get; set; }
            public string BusinessScope { get; set; }
            public string MainBusinessScope { get; set; }
            public string OrganizationPic { get; set; }
            public string LicensePic { get; set; }
            /// <summary>
            /// 0 未审核 1审核通过 2审核不通过 
            /// </summary>
            public int VerifyState { get; set; }
            public DateTime CreateTime { get; set; }
            public DateTime UpdateTime { get; set; }
            public string Remark { get; set; }

            #region API信息
            public string NotifyUrl { get; set; }
            public string DesKey { get; set; }
            public string DesIV { get; set; }
            #endregion
            #region 统计使用
            public int AppCount { get; set; }
            public int ReceiveCount { get; set; }
            public int ActivitiesCount { get; set; }
            public int ReciveCount { get; set; }
            public int ChangeCount { get; set; }
            #endregion
        }

        public class T_CompanyMore : T_Company
        {
            public string PushUrl { get; set; }
            public string ANo { get;set; }

        }
    }

    //下面这个类是针对接口2.5的，既是给活动方推送使用的
    public class API_Notify_List_Push
    {
        public class NotifyPushModForList
        {

            public string No { get; set; }
            public string Batch { get; set; }
            public string Count { get; set; }
            public ResourcesList ResourcesList { get; set; }

        }

        public class ResourcesList
        {
            public List<ResourcesContent> ResourcesContent { get; set; }
        }

        public class ResourcesContent
        {
            public string ResourcesNo { get; set; }
            public string MobileNo { get; set; }
            public int NodeCode { get; set; }
            public string NodeMsg { get; set; }
            public string ProductName { get; set; }
            public string DESC { get; set; }
            public string NodeTime { get; set; }
        }


        public class API_Accept_Log
        {
            public string Batch { get; set; }
            public DateTime AcceptTime { get; set; }
            public int DealWithCase { get; set; }
            public int ErrorCode { get; set; }
            public int Count { get; set; }
            public DateTime DealWithTime { get; set; }
            public int State { get; set; }
            public string Remark { get; set; }
            public string AcceptAllDate { get; set; }
        }

    }    


}

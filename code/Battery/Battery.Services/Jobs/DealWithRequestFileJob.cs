using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using NLog;
using static TuoKe.Services.Models.API_Notify_List;
using static TuoKe.Services.Models.API_Notify_List_Push;
using TuoKe.Services.Models;
using Newtonsoft.Json;
using System.IO;
using TuoKe.Services.Tools;

namespace TuoKe.Services.Jobs
{
    public class DealWithRequestFileJob : IJob
    {
        Logger logger = LogManager.GetLogger("Service");       
        public void Execute(IJobExecutionContext context)
        {
            var model = ConfigHelper.GetDealWithRequestFileConfig();
            object _model = context.JobDetail.JobDataMap["Model"];
            {
                List<string> ls = Dal.GetAllFile();
                //logger.Info("ls:" + ls.Count.ToString());
                if (ls.Count > 0)
                {
                    foreach (string lsstring in ls)
                    {
                        CheckNofify(model.CountToFilePath+"\\"+lsstring+".txt",model.DesKey,model.DesIV);
                    }
                }
            } 
        }


        public void CheckNofify(string pathname,string key,string iv)
        {
            try
            {
                FileStream fs = File.OpenRead(pathname);
                byte[] data = Read2Buffer(fs, 0);
                string requestData = System.Text.Encoding.UTF8.GetString(data);
                //logger.Info("content:"+ requestData.Substring(0,100));
                NotifyModForList notifymodforlist = CheckRequestDataForList(requestData,key,iv);

                if (notifymodforlist.Result.Code != 0)
                {
                    //WriteFileErr(notifymodforlist);//写入错误日志
                    UpdateFileErr(notifymodforlist.Batch, notifymodforlist.Result.Code, 1);

                }

                //即时处理
                int listcount = notifymodforlist.ResourcesList.Count;
                //logger.Info("count:" + listcount.ToString());
                if (listcount != Convert.ToInt32(notifymodforlist.Count))
                {
                    notifymodforlist.Result.Code = -5;
                    notifymodforlist.Result.ResultMsg = "数据条数不符";
                    UpdateFileErr(notifymodforlist.Batch, notifymodforlist.Result.Code, 1);

                }

                if (listcount < 1)
                {
                    notifymodforlist.Result.Code = -4;
                    notifymodforlist.Result.ResultMsg = "没有需处理的数据";
                    UpdateFileErr(notifymodforlist.Batch, notifymodforlist.Result.Code, 1);

                }
                else
                {
                    //向领取日志表写入数据
                    int res = Dal.BatchAdd(notifymodforlist.ResourcesList, notifymodforlist.No);
                    //logger.Info("res:" + res.ToString());
                    if (res > 0)
                    {
                        List<API_Notify> list_an = new List<API_Notify>();
                        string activenogroup = "";

                        //开始处理推送
                        IEnumerable<IGrouping<string, string>> query = notifymodforlist.ResourcesList.GroupBy(x => x.GroupActiveNo, x => x.GroupVerifyNo);
                        foreach (IGrouping<string, string> gnmlrl in query)
                        {
                            //用一次批量查询，找出来所有活动对应的信息，避免反复查询，提高效率
                            activenogroup += "'" + gnmlrl.Key + "',";
                        }

                        //这次循环完，会形成一个ActiveNo连成的条件字符串activenogroup
                        List<API_Notify_List.T_CompanyMore> tconditon = Dal.GetCompanyModelByActionNO(activenogroup.Substring(0, activenogroup.Length - 1));

                        foreach (API_Notify_List.T_CompanyMore item in tconditon)
                        {
                            API_Notify an = new API_Notify();
                            an.CompanyId = item.Id;
                            an.KEY = item.DesKey;
                            an.IV = item.DesIV;
                            an.CompanyUrl = item.PushUrl;
                            //an.AllData = "";

                            //下面主要是处理 API_Notify的AllData 
                            API_Notify_List_Push.NotifyPushModForList npmf = new API_Notify_List_Push.NotifyPushModForList();
                            npmf.No = item.ANo;
                            npmf.Batch = notifymodforlist.Batch;
                            npmf.Count = notifymodforlist.Count;
                            API_Notify_List_Push.ResourcesList p_rl = new API_Notify_List_Push.ResourcesList();

                            List<API_Notify_List.ResourcesList> lrl = notifymodforlist.ResourcesList.FindAll(f => f.GroupActiveNo == item.ANo).ToList();
                            if (lrl.Count > 0)
                            {
                                List<ResourcesContent> prcall = new List<ResourcesContent>();
                                Object thisLock = new Object();
                                Parallel.ForEach(lrl, ritem =>
                                {
                                    API_Notify_List_Push.ResourcesContent prc = new API_Notify_List_Push.ResourcesContent();
                                    prc.ResourcesNo = notifymodforlist.No;
                                    prc.MobileNo = ritem.bizContent.MobileNo;
                                    prc.NodeCode = ritem.bizContent.NodeCode;
                                    prc.NodeMsg = ritem.bizContent.NodeMsg;
                                    prc.ProductName = ritem.bizContent.ProductName;
                                    prc.DESC = ritem.bizContent.ProductDesc;
                                    lock (thisLock)
                                    {
                                        prcall.Add(prc);
                                    }

                                });
                                p_rl.ResourcesContent = prcall;
                            }
                            else
                            {
                                p_rl.ResourcesContent = null;
                            }
                            npmf.ResourcesList = p_rl;

                            an.AllData = Newtonsoft.Json.JsonConvert.SerializeObject(npmf);
                            list_an.Add(an);
                        }


                        //开始写入API_Notify表
                        int res1 = Dal.BatchAddNotify(list_an);
                        if (res1 > 0)
                        {
                            notifymodforlist.Result.Code = 0;
                            notifymodforlist.Result.ResultMsg = "执行成功";

                            UpdateFileErr(notifymodforlist.Batch, 0, 1);

                        }
                        else
                        {
                            notifymodforlist.Result.Code = res1;
                            notifymodforlist.Result.ResultMsg = ("写入数据库错误");
                            UpdateFileErr(notifymodforlist.Batch, res1, 1);//更新日志

                        }
                    }
                    else if (res == -6)
                    {
                        notifymodforlist.Result.Code = -6;
                        notifymodforlist.Result.ResultMsg = "该批次已处理过了";
                        //UpdateFileErr(notifymodforlist.Batch, notifymodforlist.Result.Code, 1);//更新日志

                    }
                    else
                    {
                        notifymodforlist.Result.Code = res;
                        notifymodforlist.Result.ResultMsg = (res == -2 ? "资源不存在" : res == -3 ? "手机号码不存在" : "");
                        UpdateFileErr(notifymodforlist.Batch, notifymodforlist.Result.Code, 1);//更新日志

                    }
                }

            }
            catch (Exception e)
            {
                logger.Error(e, e.StackTrace);
                NotifyMod checkData = new NotifyMod();
                checkData.Result.Code = -1;
                checkData.Result.ResultMsg = "回调失败!";

            }
        }

        public NotifyModForList CheckRequestDataForList(string requestData,string key,string iv)
        {
            NotifyModForList Notify = new NotifyModForList();
            Notify.Result.Code = -1;
            Notify.Result.ResultMsg = "数据格式有误";

            try
            {
                var dataDynamic = JsonConvert.DeserializeObject<CheckModForList>(requestData);
                Notify.Batch = dataDynamic.Batch;
                Notify.Count = dataDynamic.Count;

                //检查一下该批次是不是已经处理过了
                API_Accept_Log haveAAL = Dal.GetAPI_Accept_LogModel(Notify.Batch);
                if (haveAAL != null)
                {
                    Notify.Result.Code = -6;
                    Notify.Result.ResultMsg = "该批次已处理过了";
                    //UpdateFileErr(Notify.Batch, Notify.Result.Code, 1);
                }
                else
                {
             
                    if (!string.IsNullOrEmpty(dataDynamic.No) && !string.IsNullOrEmpty(dataDynamic.ResourcesList.ToString()))
                    {
                        string RNo = Notify.No = dataDynamic.No;

                        T_Company model = Dal.GetCompanyModel(RNo);
                        if (model != null)
                        {
                            Object thisLock = new Object();
                            List<API_Notify_List.ResourcesList> list_ar = new List<API_Notify_List.ResourcesList>();

                            Parallel.ForEach(dataDynamic.ResourcesList, item => {

                                API_Notify_List.ResourcesList ar = new API_Notify_List.ResourcesList();
                                tkContent tkc = new tkContent();

                                if (!string.IsNullOrEmpty(item.tkContent))
                                    tkc = JsonConvert.DeserializeObject<tkContent>(DesHelper.DecryptString_lv(item.tkContent, key, iv));

                                string desKey = model.DesKey, desIV = model.DesIV;
                                string resourcesData = DesHelper.DecryptString_lv(item.bizContent, desKey, desIV);
                                API_Notify_List.bizContent bizDynamic = JsonConvert.DeserializeObject<API_Notify_List.bizContent>(resourcesData);
                                ar.GroupActiveNo = tkc.ActiveNo;
                                ar.GroupVerifyNo = tkc.VerifyNo;
                                ar.tkContent = tkc;
                                ar.bizContent = bizDynamic;
                                lock (thisLock)
                                {
                                    list_ar.Add(ar);
                                }
                            });

                            //foreach (ResourcesListTemp item in dataDynamic.ResourcesList)
                            //{
                            //    API_Notify_List.ResourcesList ar = new API_Notify_List.ResourcesList();
                            //    tkContent tkc = new tkContent();

                            //    if (!string.IsNullOrEmpty(item.tkContent))
                            //        tkc = JsonConvert.DeserializeObject<tkContent>(XT.MVC.Core.Encrypt.DesHelper.DecryptString_lv(item.tkContent, XT.MVC.Core.Common.ConfigHelper.GetBranch("TuoKe.DesKey"), XT.MVC.Core.Common.ConfigHelper.GetBranch("TuoKe.DesIV")));

                            //    string desKey = model.DesKey, desIV = model.DesIV;
                            //    string resourcesData = XT.MVC.Core.Encrypt.DesHelper.DecryptString_lv(item.bizContent, desKey, desIV);
                            //    API_Notify_List.bizContent bizDynamic = JsonConvert.DeserializeObject<API_Notify_List.bizContent>(resourcesData);
                            //    ar.GroupActiveNo = tkc.ActiveNo;
                            //    ar.GroupVerifyNo = tkc.VerifyNo;
                            //    ar.tkContent = tkc;
                            //    ar.bizContent = bizDynamic;
                            //    list_ar.Add(ar);
                            //}


                            Notify.ResourcesList = list_ar;
                            Notify.Result.Code = 0;
                            Notify.Result.ResultMsg = "";
                        }
                        else
                        {
                            Notify.Result.Code = -2;
                            Notify.Result.ResultMsg = "资源不存在";
                            UpdateFileErr(Notify.Batch, Notify.Result.Code, 1);
                            //WriteFileErr(notifymodforlist);//写入错误日志
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                logger.Error(e, e.StackTrace);
            }
            return Notify;
        }

        /// <summary>  
        /// 读入文件   
        /// </summary>  
        /// <param name="Content"></param>  
        /// <param name="FileSavePath"></param>  
        public static byte[] Read2Buffer(Stream stream, int BufferLen)
        {
            // 如果指定的无效长度的缓冲区，则指定一个默认的长度作为缓存大小
            if (BufferLen < 1)
            {
                BufferLen = 0x8000;
            }
            // 初始化一个缓存区
            byte[] buffer = new byte[BufferLen];
            int read = 0;
            int block;

            // 每次从流中读取缓存大小的数据，知道读取完所有的流为止
            while ((block = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                // 重新设定读取位置
                read += block;
                // 检查是否到达了缓存的边界，检查是否还有可以读取的信息
                if (read == buffer.Length)
                {
                    // 尝试读取一个字节
                    int nextByte = stream.ReadByte();
                    // 读取失败则说明读取完成可以返回结果
                    if (nextByte == -1)
                    {
                        return buffer;
                    }
                    // 调整数组大小准备继续读取
                    byte[] newBuf = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuf, buffer.Length);
                    newBuf[read] = (byte)nextByte;
                    buffer = newBuf;// buffer是一个引用（指针），这里意在重新设定buffer指针指向一个更大的内存
                    read++;
                }
            }
            // 如果缓存太大则使用ret来收缩前面while读取的buffer，然后直接返回
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }

        //接收的数据出现错误写入日志的方法
        public static void WriteFileErr(NotifyModForList notifymodforlist)
        {
            API_Accept_Log aal = new API_Accept_Log
            {
                Batch = notifymodforlist.Batch,
                AcceptTime = DateTime.Now,
                DealWithCase = 0,
                ErrorCode = notifymodforlist.Result.Code,
                Count = Convert.ToInt32(notifymodforlist.Count),
                DealWithTime = DateTime.Now,
                State = 0,
                Remark = notifymodforlist.Result.ResultMsg
            };
            int r_s = Dal.AddAPI_Accept_Log(aal);
        }

        //更新处理结果日志的方法
        public static void UpdateFileErr(string batch, int errorCode, int state)
        {
            int r_ufe = Dal.UpdateAPI_Accept_Log(batch, errorCode, state);
        }
    }
}

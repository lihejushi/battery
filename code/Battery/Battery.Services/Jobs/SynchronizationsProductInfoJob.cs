using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using NLog;
using Newtonsoft.Json;
using TuoKe.Services.Tools;
using TuoKe.Services.Models;

namespace TuoKe.Services.Jobs
{
    public class SynchronizationsProductInfoJob:IJob
    {
        Logger logger = LogManager.GetLogger("Service");
        public void Execute(IJobExecutionContext context)
        {
            
            try
            { 
                object _model = context.JobDetail.JobDataMap["Model"];
                {
                    #region
                    var model = ConfigHelper.GetSynchronizationsProductInfoConfig();
                    List<API_Notify> list_an = Dal.GetNotify();
                    StringBuilder sqlstring = new StringBuilder();
                    
                    foreach (API_Notify item in list_an)
                    {
                        try
                        {
                            if ((item.AllData != DBNull.Value.ToString() && item.AllData != string.Empty) && (item.IV != DBNull.Value.ToString() && item.IV != string.Empty) && (item.KEY != DBNull.Value.ToString() && item.KEY != string.Empty))
                            {
                                if (item.SendCount < model.TotelCount)
                                {
                                    TimeSpan ts = DateTime.Now - item.SendTime;
                                    int invertal_time = Convert.ToInt32(ts.TotalMinutes);
                                    if (invertal_time >= model.Interval)
                                    {
                                        
                                        string senddata = DesHelper.EncryptString_Iv(item.AllData, item.KEY, item.IV);//DES加密
                                        //logger.Info("2:" + senddata);
                                        string reponseRet = (new RequestHelper()).DoPostMethodNotify(item.CompanyUrl, senddata);
                                        //logger.Info(reponseRet);
                                        string tsqlstring = " UPDATE API_Notify SET SendTime='" + DateTime.Now.ToString() + "',SendCount=ISNULL(SendCount,0)+1";
                                        if (reponseRet == "success" || model.TotelCount <= item.SendCount + 1)
                                        {
                                            tsqlstring += ",State=1,Reponse='" + reponseRet + "',ReponseTime='" + DateTime.Now.ToString() + "'";
                                        }
                                        tsqlstring += " WHERE Id=" + item.Id.ToString()+";";
                                        tsqlstring += "INSERT INTO API_Notify_Log(CompanyId,[KEY],IV,CompanyUrl,AllData,SendTime,SendCount,Reponse,ReponseTime,ErrMessage) ";
                                        tsqlstring += "VALUES(" + item.CompanyId + ",'" + item.KEY + "','" + item.IV + "','" + item.CompanyUrl + "','" + item.AllData + "','" + DateTime.Now.ToString() + "'," + (item.SendCount + 1).ToString() + ",'" + reponseRet + "','" + DateTime.Now.ToString() + "','')";
                                        sqlstring.Append(tsqlstring);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string  errtsqlstring = "INSERT INTO API_Notify_Log(CompanyId,[KEY],IV,CompanyUrl,AllData,SendTime,SendCount,Reponse,ReponseTime,ErrMessage) ";
                            errtsqlstring += "VALUES(" + item.CompanyId + ",'" + item.KEY + "','" + item.IV + "','" + item.CompanyUrl + "','" + item.AllData + "','" + DateTime.Now.ToString() + "'," + (item.SendCount + 1).ToString() + ",'','" + DateTime.Now.ToString() + "','" + ex.ToString() + "')";
                            sqlstring.Append(errtsqlstring);
                        }
                    }

                    string othersql = @"
                            BEGIN TRAN
                            BEGIN TRY
                        ";
                    othersql += sqlstring.ToString();
                    othersql+= @"    
                            SELECT @@ROWCOUNT;
                            COMMIT TRAN
                          
                        END TRY
                        BEGIN CATCH
                            SELECT 0;
                            ROLLBACK TRAN 
                        END CATCH 
                        ";
                    //logger.Info(othersql);
                    int r = Dal.AfterNotify(othersql);

                }
                #endregion
                
            }
            catch (Exception ex)
            {
                logger.Error("SynchronizationsProductInfoJob：" + ex.Message);
                logger.Error("SynchronizationsProductInfoJob：" + ex.StackTrace);
            }
        }
    }
}

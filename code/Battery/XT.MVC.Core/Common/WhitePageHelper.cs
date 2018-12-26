using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using XT.MVC.Core.Caching;
using XT.MVC.Core.Infrastructure;
using XT.MVC.Core.Web;

namespace XT.MVC.Core.Common
{
    /// <summary>
    ///  IP白名单
    /// </summary>
    public class WhitePageHelper
    {
        /// <summary>
        ///  获取IP白名单列表
        /// </summary>
        public static List<string> WhitePageList(string ipWhitePath)
        {
            List<string> whiteList = new List<string>( );
            object obj;
            var cache = EngineContext.Current.Resolve<ICacheManager>();
            obj = cache.Get<object>(ipWhitePath);
            if( obj == null )
            {
                using (StreamReader sr = new StreamReader(ipWhitePath, Encoding.Default))
                {
                    String line;
                    while( (line = sr.ReadLine( )) != null )
                        if( string.IsNullOrEmpty( line ) == false )
                            whiteList.Add( line );
                    cache.Set(ipWhitePath, whiteList, 60 * 1000 * 100);
                    sr.Close( );
                }
            }
            else
                whiteList = (List<string>)obj;
            return whiteList;
        }

        /// <summary>
        ///  判断IP是否包含在名单内
        /// </summary>
        public static bool IsWhilte( string whiteFilePath, string content )
        {
            List<string> wlist = WhitePageList(whiteFilePath);
            return wlist.Contains( content );
        }
        /// <summary>
        ///  判断IP是否包含在名单内
        /// </summary>
        public static bool IsWhilteIP(string whiteFilePath)
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            return IsWhilte(whiteFilePath, webHelper.GetCurrentIpAddress());
        }
    }//class end
}

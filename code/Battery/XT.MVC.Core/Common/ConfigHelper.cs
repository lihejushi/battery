using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using XT.MVC.Core.Caching;
using XT.MVC.Core.Infrastructure;

namespace XT.MVC.Core.Common
{
    /// <summary>
    ///  应用程序级配置项
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        ///  获取AppSetting设置值
        /// </summary>
        /// <param name="key">键</param>
        public static string GetAppConfig( string key )
        {
            return ConfigurationManager.AppSettings[key];
        }
        /// <summary>
        ///  获取ConnectionSetting设置值
        /// </summary>
        /// <param name="item">设置键</param>
        /// <returns></returns>
        public static string GetConnectionConfig( string key )
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        #region branchPath

        /// <summary>
        ///  获取分支配置（Web项目）
        /// </summary>
        /// <param name="key">键值</param>
        public static string GetBranch(  string key )
        {
            return IsWebApplication ? GetBranchByWeb(key) : GetBranchByApplication(key);
        }

        /// <summary>
        ///  获取分支配置（Web项目）
        /// </summary>
        /// <param name="key">键值</param>
        public static string GetBranchByWeb(  string key )
        {
            return GetBranchValue("\\App_Data\\Init.Config", key, true);
        }
        /// <summary>
        ///  获取分支配置（桌面项目）
        /// </summary>
        /// <param name="key">键值</param>
        public static string GetBranchByApplication( string key )
        {
            //string branchPath = ConfigHelper.GetAppConfig( "XT.Branch" );

            return GetBranchValue("\\App_Data\\Init.Config", key);
        }
        /// <summary>
        ///  获取分支配置
        /// </summary>
        /// <param name="branchPath">分支路径</param>
        /// <param name="key">键值</param>
        ///<param name="relativePath">是否相对于当前应用程序路径</param>
        public static string GetBranchValue( string branchPath, string key, bool relativePath = true )
        {
            Dictionary<string, string> dicts = GetBranchConfig( branchPath, relativePath );
            if( dicts != null && dicts.ContainsKey( key ) == true )
                return dicts[key];
            else
                return "";
        }
        /// <summary>
        ///  获取配置项字典
        /// </summary>
        ///<param name="branchPath">分支路径</param>
        ///<param name="relativePath">是否相对于当前应用程序路径</param>
        public static Dictionary<string, string> GetBranchConfig( string branchPath, bool relativePath = true )
        {
            string _path = branchPath;
            if(relativePath==true)
                _path = BasePath + branchPath;                
            //分支文件不存在，直接返回
            if( File.Exists( _path ) == false )
                return null;

            Dictionary<string, string> branchDicts = null;
            var cache = EngineContext.Current.Resolve<ICacheManager>();
            branchDicts = cache.Get<Dictionary<string, string>>(_path);
            if (branchDicts == null || branchDicts.Count == 0)
            {
                branchDicts = new Dictionary<string, string>( );
                using( StreamReader sr = new StreamReader( _path, Encoding.Default ) )
                {
                    string line = sr.ReadLine( );
                    while( string.IsNullOrEmpty( line = sr.ReadLine( ) ) == false )
                    {
                        //以#开头的为注释
                        if( line.StartsWith( "#" ) == true )
                            continue;
                        int findex = line.IndexOf( "=" );
                        if( findex > 0 )
                            branchDicts.Add( line.Substring( 0, findex ), line.Substring( findex + 1 ) );
                    }

                    cache.Set(_path, branchDicts, 60 * 1000 * 100);
                    sr.Close( );
                }
            }

            return branchDicts;
        }
        #endregion

        #region 应用程序基础路径
        private static string _BaseDir;
        private static object _BaseDir_Lock = new object( );
        /// <summary>
        ///  应用程序基础路径
        /// </summary>
        public static string BasePath
        {
            get
            {
                if( !string.IsNullOrEmpty( _BaseDir ) )
                {
                    return _BaseDir;
                }
                lock( _BaseDir_Lock )
                {
                    if( string.IsNullOrEmpty( _BaseDir ) )
                    {
                        string path = string.Empty;
                        if( HttpContext.Current != null )
                        {
                            path = HttpContext.Current.Server.MapPath( "~/" );
                        }
                        else
                        {
                            path = AppDomain.CurrentDomain.BaseDirectory;
                            if( !string.IsNullOrEmpty( AppDomain.CurrentDomain.RelativeSearchPath ) )
                            {
                                path = Path.Combine( path, AppDomain.CurrentDomain.RelativeSearchPath );
                            }
                            if( path.ToLower( ).EndsWith( @"\bin" ) )
                            {
                                string str2 = path.Substring( 0, path.Length - "bin".Length );
                                if( File.Exists( Path.Combine( str2, "web.config" ) ) )
                                {
                                    path = str2;
                                }
                            }
                        }
                        if( !(string.IsNullOrEmpty( path ) || !(path.Substring( path.Length - 1, 1 ) != @"\")) )
                        {
                            path = path + @"\";
                        }
                        _BaseDir = path;
                    }
                    return _BaseDir;
                }
            }
        }

        public static bool IsWebApplication
        {
            get
            {
                return string.Compare(System.IO.Path.GetFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile), "Web.config", StringComparison.OrdinalIgnoreCase) > 0;
            }
        }
        #endregion
    }//class end
}
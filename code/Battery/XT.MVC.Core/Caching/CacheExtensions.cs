using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XT.MVC.Core.Caching
{
    public static class CacheExtensions
    {
        /// <summary>
        /// 获取与指定的键相关联的值，默认缓存60秒
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="cacheManager"></param>
        /// <param name="key">指定的键</param>
        /// <param name="acquire">获取方法</param>
        /// <returns>值</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 60, acquire);
        }

        /// <summary>
        /// 获取与指定的键相关联的值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="cacheManager"></param>
        /// <param name="key">指定的键</param>
        /// <param name="cacheTime">缓存时间</param>
        /// <param name="acquire">获取方法</param>
        /// <returns>值</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire)
        {
            //如果设置了缓存，则返回已设定的缓存
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }
            else
            {
                //如果没有设置缓存，通过获取方法添加缓存
                var result = acquire();
                cacheManager.Set(key, result, cacheTime);
                return result;
            }
        }
    }
}

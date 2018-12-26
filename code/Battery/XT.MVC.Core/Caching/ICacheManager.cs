using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XT.MVC.Core.Caching
{
    /// <summary>
    /// 缓存管理器接口
    /// </summary>
    public interface ICacheManager
    {
        List<string> Keys();

        /// <summary>
        /// 获取与指定的键相关联的值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">指定的键.</param>
        /// <returns>与指定的键相关联的值.</returns>
        T Get<T>(string key);

        /// <summary>
        /// 将指定的键和对象添加到缓存中。
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <param name="data">对象</param>
        /// <param name="cacheTime">缓存时长</param>
        void Set(string key, object data, int cacheTime);

        /// <summary>
        /// 获取一个值,该值指示值是否与指定的键相关联的是缓存
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <returns>true 存在，false 不存在</returns>
        bool IsSet(string key);

        /// <summary>
        /// 移除指定键的缓存
        /// </summary>
        /// <param name="key">指定键</param>
        void Remove(string key);

        /// <summary>
        /// 移除指定匹配项的缓存
        /// </summary>
        /// <param name="pattern">匹配表达式</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        void Clear();
    }
}

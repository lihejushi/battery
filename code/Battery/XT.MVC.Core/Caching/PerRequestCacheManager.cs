using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace XT.MVC.Core.Caching
{
    /// <summary>
    /// 静态缓存
    /// </summary>
    public partial class PerRequestCacheManager : ICacheManager
    {
        private readonly HttpContextBase _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">Web上下文</param>
        public PerRequestCacheManager(HttpContextBase context)
        {
            this._context = context;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        protected virtual IDictionary GetItems()
        {
            if (_context != null)
                return _context.Items;

            return null;
        }

        /// <summary>
        /// 获取与指定的键相关联的值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">指定的键.</param>
        /// <returns>与指定的键相关联的值.</returns>
        public virtual T Get<T>(string key)
        {
            var items = GetItems();
            if (items == null)
                return default(T);

            return (T)items[key];
        }

        /// <summary>
        /// 将指定的键和对象添加到缓存中。
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <param name="data">对象</param>
        /// <param name="cacheTime">缓存时长</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            var items = GetItems();
            if (items == null)
                return;

            if (data != null)
            {
                if (items.Contains(key))
                    items[key] = data;
                else
                    items.Add(key, data);
            }
        }

        /// <summary>
        /// 获取一个值,该值指示值是否与指定的键相关联的是缓存
        /// </summary>
        /// <param name="key">指定的键</param>
        /// <returns>true 存在，false 不存在</returns>
        public virtual bool IsSet(string key)
        {
            var items = GetItems();
            if (items == null)
                return false;

            return (items[key] != null);
        }

        /// <summary>
        /// 移除指定键的缓存
        /// </summary>
        /// <param name="key">指定键</param>
        public virtual void Remove(string key)
        {
            var items = GetItems();
            if (items == null)
                return;

            items.Remove(key);
        }

        /// <summary>
        /// 移除指定匹配项的缓存
        /// </summary>
        /// <param name="pattern">匹配表达式</param>
        public virtual void RemoveByPattern(string pattern)
        {
            var items = GetItems();
            if (items == null)
                return;

            var enumerator = items.GetEnumerator();
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();
            while (enumerator.MoveNext())
            {
                if (regex.IsMatch(enumerator.Key.ToString()))
                {
                    keysToRemove.Add(enumerator.Key.ToString());
                }
            }

            foreach (string key in keysToRemove)
            {
                items.Remove(key);
            }
        }

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        public virtual void Clear()
        {
            var items = GetItems();
            if (items == null)
                return;

            var enumerator = items.GetEnumerator();
            var keysToRemove = new List<String>();
            while (enumerator.MoveNext())
            {
                keysToRemove.Add(enumerator.Key.ToString());
            }

            foreach (string key in keysToRemove)
            {
                items.Remove(key);
            }
        }

        public List<string> Keys()
        {
            var items = GetItems();
            if (items == null)
                return new List<string>();
            var enumerator = items.GetEnumerator();
            var keys = new List<String>();
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }
            return keys;
        }
    }
}

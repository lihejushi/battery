using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XT.MVC.Core.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// 是否是默认值或者null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }
    }
}

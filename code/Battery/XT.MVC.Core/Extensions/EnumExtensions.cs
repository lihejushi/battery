using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XT.MVC.Core.Extensions
{
    public static class EnumExtensions
    {
        #region Enum
        /// <summary>
        ///  获取Enum的名称（小写）
        /// </summary>
        public static string GetName(this Enum e)
        {
            return GetName(e, true);
        }
        /// <summary>
        ///  获取Enum原始名称
        /// </summary>
        public static string GetOriName(this Enum e)
        {
            return GetName(e, false);
        }
        /// <summary>
        ///  获取Enum的名称
        /// </summary>
        /// <param name="tolower">是否转为小写</param>
        public static string GetName(this Enum e, bool tolower)
        {
            string name = Enum.GetName(e.GetType(), e);
            if (tolower == true)
                name = name.ToLower();
            return name;
        }
        #endregion
    }
}

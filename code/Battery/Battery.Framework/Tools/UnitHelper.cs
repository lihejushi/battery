using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Framework.Tools
{
    public class UnitHelper
    {
        /// <summary>
        /// 流量从k转化为合适的单位
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static string FormKb(long k)
        {
            if (k < 1024) return k.ToString() + "KB";
            else if (k > 1024 * 1024) return (Math.Floor(Convert.ToDouble(k) / (1024 * 1024) * 100) / 100).ToString() + "GB";
            else return (Math.Floor(Convert.ToDouble(k) / 1024 * 100) / 100).ToString() + "MB";
        }

        /// <summary>
        /// 流量从m转化为合适的单位
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public static string FormMb(long m)
        {
            if (m < 1024) return m.ToString() + "MB";
            else return (Math.Floor(Convert.ToDouble(m) / 1024 * 100) / 100).ToString() + "MB";
        }


    }
}

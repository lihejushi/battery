using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XT.MVC.Core.Extensions
{
    public static class StrExtensions
    {
        /// <summary>
        /// 验证订阅者邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string EnsureSubscriberEmailOrThrow(this string email)
        {
            string output = email ?? string.Empty;
            output = output.Trim();
            output = EnsureMaximumLength(output, 255);

            if (!IsValidEmail(output))
            {
                throw new Exception("Email is not valid.");
            }

            return output;
        }

        /// <summary>
        /// 验证有效的电子邮件格式的字符串
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns></returns>
        public static bool IsValidEmail(this string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            email = email.Trim();
            var result = Regex.IsMatch(email, "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.IgnoreCase);
            return result;
        }

        /// <summary>
        /// 确保字符串不超过最大允许长度
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <param name="maxLength">最大长度</param>
        /// <param name="postfix">如果超过最大长度，添加后缀</param>
        /// <returns></returns>
        public static string EnsureMaximumLength(this string str, int maxLength, string postfix = null)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
            {
                var result = str.Substring(0, maxLength);
                if (!String.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }
                return result;
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 确定字符串中只有数字
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <returns>输入字符串只有数值的部分</returns>
        public static string EnsureNumericOnly(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return string.Empty;

            var result = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        ///  将字符串，按指定元素分隔成int数组
        /// </summary>
        public static int[] ToIntArray(this string s, char[] separator)
        {
            int[] array = null;
            //分隔字符串
            if (s != null && string.IsNullOrEmpty(s.Trim()) == false)
            {
                string[] split = s.Split(separator);
                if (split.Length > 0)
                {
                    int arrayc = 0, itemp = 0;
                    array = new int[split.Length];
                    for (int i = 0, c = split.Length; i < c; i++)
                        if (int.TryParse(split[i], out itemp))
                        {
                            array[i] = itemp;
                            arrayc++;
                        }
                }
            }
            return array ?? new int[] { };
        }
        /// <summary>
        ///  将字符串转换成指定的Enum类型，不分大小写
        /// </summary>
        /// <param name="type">Enum类型</param>
        /// <returns></returns>
        public static object ToEnum(this string s, Type type)
        {
            // 传递的不是enum类型，直接返回null
            if (type.IsEnum == false)
                return null;
            try
            {
                s = s.Trim();
                object obj = Enum.Parse(type, s, true);
                return obj;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        ///  将字符串转换为int，如果不成功，指定为传递的默认值
        /// </summary>
        /// <param name="d">不成功时的默认值</param>
        public static int ToInt(this string s, int d)
        {
            int i = d;
            if (int.TryParse(s, out i) == false)
                i = d;
            return i;
        }

        public static string FormatMoblie(this string obj)
        {
            return string.IsNullOrEmpty(obj) ? "" : obj.Remove(3) + "****" + obj.Substring(obj.Length - 4, 4);
        }

        #region Xml 字符编码
        /// <summary>
        ///  Xml 字符编码
        /// </summary>
        public static string XmlEscape(this string str)
        {
            return str.Replace("&", "&amp;").Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("'", "&apos;")
                .Replace("\"", "&quot;");
        }
        #endregion
    }
}

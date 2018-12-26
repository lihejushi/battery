using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XT.MVC.Core
{
    public static class CoreVersion
    {
        /// <summary>
        /// 当前代码版本
        /// </summary>
        public static string CurrentVersion
        {
            get
            {
                return string.Format("{0}.{1}.{2}", FileMajorPart, FileMinorPart, FileBuildPart);
            }
        }

        public static int FileMajorPart { get { return 1; } }
        public static int FileMinorPart { get { return 0; } }
        public static int FileBuildPart { get { return 0; } }
    }
}

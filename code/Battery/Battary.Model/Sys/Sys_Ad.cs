using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model.Sys
{
    [Serializable]
    public class Sys_Ad : BasicModel
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 类型 1首页轮播图
        /// </summary>
        public int AdType { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图片url
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 导航内容
        /// </summary>
        public string NavJson { get; set; }
        public string Url { get; set; }
    }

    [Serializable]
    public class AdType : BasicModel
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }
}

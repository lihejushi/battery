using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model
{
    [Serializable]
    public class V_NavData
    {
        public int AppId { get; set; }
        public string NavImg { get; set; }
        public string NavContentType { get; set; }
        public string NavContentValue { get; set; }
        public string NavLinkType { get; set; }
        public string NavLink { get; set; }
        public string NavValueTitle { get; set; }
        public string NavValue { get; set; }
    }
}

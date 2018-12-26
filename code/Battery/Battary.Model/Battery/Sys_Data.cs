using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model.Battery
{
    public class Sys_Data: BasicModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string TypeCode { get; set; }
        public int State { get; set; }
        public int Sort { get; set; }
    }
}

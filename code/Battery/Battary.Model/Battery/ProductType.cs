using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Battery.Model.Battery
{
    public class ProductType:BasicModel
    {
        public int ID { get; set; }
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeT { get; set; }
        public string Remark { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float High { get; set; }
        public float Weight { get; set; }
        public int State { get; set; }
    }
}

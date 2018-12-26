using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Model.Battery
{
    public class ProductInstall:BasicModel
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public string Num { get; set; }
        public int StoreID { get; set; }
        public int StoreName { get; set; }
        public int StaffID { get; set; }
        public string StaffName { get; set; }
        public string StaffNum { get; set; }
        public DateTime InstallDate { get; set; }
        public DateTime? ActiveDate { get; set; }
        public int InstallTimes { get; set; }
        public decimal InstallCost { get; set; }
        public int InstallOrUn { get; set; }
        public int IsScrap { get; set; }
        public string Remark { get; set; }
    }
}

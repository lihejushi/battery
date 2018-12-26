using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Battery.Model.Battery
{
    public class Products: BasicModel
    {
        public int ID { get; set; }
        public string Num { get; set; }
        public string DeCode { get; set; }
        public int TypeID { get; set; }
        public string Voltage { get; set; }
        public string Area { get; set; }
        public string UserArea { get; set; }
        public int TypeFor { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? ScrapDate { get; set; }
        public DateTime? RealScrapDate { get; set; }
        public string ActivationCode { get; set; }
        public int? BackStoreID { get; set; }
        public int? BackStaffID { get; set; }
        public string BackStaffNum { get; set; }
        public DateTime? BackDate { get; set; }
        public string Guarantee { get; set; }
        public int State { get; set; }
        public string StateT { get; set; }
        public string BackStaffName { get; set; }
        public string TypeName { get; set; }
        public string Factory { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeT { get; set; }
        public int LibraryID { get; set; }
        public string LibraryName { get; set; }
    }

    public class EndResult
    {
        public string ActivationCode { get; set; }
        public int EndString { get; set; }
    }
}

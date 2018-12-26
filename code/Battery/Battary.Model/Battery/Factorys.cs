using System;
using System.Collections.Generic;
using System.Linq;


namespace Battery.Model.Battery
{
    public class Factorys:BasicModel
    {
        public int ID { get; set; }
        public string FactoryNum { get; set; }
        public string Factory { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Corporate { get; set; }
        public int CorporateID { get; set; }
        public string License { get; set; }
        public string Manager { get; set; }
        public string ManagerPhone { get; set; }
        public DateTime BeginTime { get; set; }
        public string LicenseImg { get; set; }
        public int State { get; set; }
    }
}

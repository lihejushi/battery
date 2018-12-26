using System;
using System.Collections.Generic;
using System.Linq;


namespace Battery.Model.Battery
{
    public class ProductSend: BasicModel
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public string Num { get; set; }
        public string SourcePlace { get; set; }
        public string TargetPlace { get; set; }
        public int Type { get; set; }
        public string Introduce { get; set; }
        public string Note { get; set; }
        public int Transport { get; set; }
        public DateTime TransportTime { get; set; }
        public string TransportNum { get; set; }
        public DateTime AcceptTime { get; set; }
        public int TransportStoreID { get; set; }
        public int AcceptStoreID { get; set; }
    }
}

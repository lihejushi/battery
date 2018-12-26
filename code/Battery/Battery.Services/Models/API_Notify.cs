using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuoKe.Services.Models
{
    public class API_Notify
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string KEY { get; set; }
        public string IV { get; set; }
        public string CompanyUrl { get; set; }
        public string AllData { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime SendTime { get; set; }
        public int SendCount { get; set; }
        public string Reponse { get; set; }
        public DateTime? ReponseTime { get; set; }
        public int State { get; set; }
    }
}

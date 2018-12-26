using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Support.Service.Models
{
    public class TokenConfig
    {
        public string DbConnectionStr { get; set; }
        public string Exec { get; set; }
        public string AppId { get; set; }
        public string AppSecretKey { get; set; }
        public string TokenType { get; set; }
    }
}

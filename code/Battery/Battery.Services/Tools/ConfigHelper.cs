using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuoKe.Services.Models;
using System.IO;

namespace TuoKe.Services.Tools
{
    public class ConfigHelper
    {
        public class ToukeConfig
        {
            public SynchronizationsProductInfo SynchronizationsProductInfoConfigs { get; set; }

            public DealWithRequestFile DealWithRequestFileConfigs { get; set; }

            public static ToukeConfig GetConfig()
            {
                string filePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config.json");
                var configs = Newtonsoft.Json.JsonConvert.DeserializeObject<ToukeConfig>(File.ReadAllText(filePath));
                return configs;
            }

            public static void SaveConfig(ToukeConfig model)
            {
                string filePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config.json");
                File.AppendAllText(filePath, Newtonsoft.Json.JsonConvert.SerializeObject(model));
            }
        }

        public static SynchronizationsProductInfo GetSynchronizationsProductInfoConfig()
        {
            return ToukeConfig.GetConfig().SynchronizationsProductInfoConfigs;
        }

        public static DealWithRequestFile GetDealWithRequestFileConfig()
        {
            return ToukeConfig.GetConfig().DealWithRequestFileConfigs;
        }
    }
}

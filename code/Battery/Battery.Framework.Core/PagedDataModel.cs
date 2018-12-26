using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Battery.Framework.Core
{
    public class PagedDataModel<T>
    {
        public PagedDataModel(List<T> data, int draw, int pageSize, object attach = null)
            : this(data, draw, pageSize, pageSize, attach)
        {

        }
        public PagedDataModel(List<T> data, int draw, int records, int filteredRecords, object attach = null)
        {
            this.Data = data;
            this.Draw = draw;
            this.RecordsTotal = records;
            this.RecordsFiltered = records;
            this.Attach = attach;
        }

        [JsonProperty("data")]
        public List<T> Data { get; set; }
        [JsonProperty("draw")]
        public int Draw { get; set; }
        [JsonProperty("recordsFiltered")]
        public int RecordsFiltered { get; set; }
        [JsonProperty("recordsTotal")]
        public int RecordsTotal { get; set; }
        [JsonProperty("attach",NullValueHandling= NullValueHandling.Ignore)]
        public object Attach { get; set; }
    }
}

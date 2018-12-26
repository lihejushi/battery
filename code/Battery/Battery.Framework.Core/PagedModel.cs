using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Battery.Framework.Core
{
    public class PagedModel
    {
        //public class Column
        //{
        //    public string data { get; set; }
        //    public string name { get; set; }
        //    public bool searchable { get; set; }
        //    public bool orderable { get; set; }
        //    public SearchInfo search { get; set; }
        //}
        //public class SearchInfo
        //{
        //    public string value { get; set; }
        //    public bool regex { get; set; }
        //}
        //public class Order
        //{
        //    public int column { get; set; }
        //    public string dir { get; set; }
        //}


        public PagedModel()
        {
            Draw = 1;
            PageStart = 0;
            PageLength = 20;
            Columns = Orders = string.Empty;
        }

        public int Draw { get; set; }

        private int _pageStart = 0;

        public int PageStart
        {
            get { return _pageStart + 1; }
            set { _pageStart = value; }
        }
        private int _pageLength = 20;

        public int PageLength
        {
            get { return _pageLength - 1; }
            set { _pageLength = value; }
        }

        public string Columns { get; set; }
        public string Orders { get; set; }

        public List<string> GetColumns()
        {
            if (string.IsNullOrEmpty(Columns)) return new List<string>();
            else
            {
                return Columns.Split(',').Where(m => string.IsNullOrEmpty(m) == false).Select(m => m.Trim()).ToList();
            }
        }

        public Dictionary<string,string> GetOrders()
        {
            var columns = GetColumns();
            var orders = (Orders ?? string.Empty).Split(',')
                .Where(m => string.IsNullOrEmpty(m) == false && Regex.IsMatch(m, ""))
                .Select(m =>
                {
                    var _t = m.Split('|');
                    return new
                    {
                        ColumnNo = Convert.ToInt32(_t[0]),
                        OrderBy = _t[1] == "desc" ? "DESC" : "ASC"
                    };
                }).Where(m => columns.Count > m.ColumnNo && m.ColumnNo >= 0).ToList();

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var order in orders)
            {
                if (dict.ContainsKey(columns[order.ColumnNo]) == false)
                {
                    dict.Add(columns[order.ColumnNo], order.OrderBy);
                }
            }
            return dict;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Battery.Framework.Tools
{
    public class DataTablesRequest
    {
        private HttpRequestBase request;
        private const string sEchoParameter = "sEcho";
        private const string iDisplayStartParameter = "iDisplayStart";
        private const string iDisplayLengthParameter = "iDisplayLength";
        private const string iColumnsParameter = "iColumns";
        private const string sColumnsParameter = "sColumns";
        private const string iSortingColsParameter = "iSortingCols";
        private const string iSortColPrefixParameter = "iSortCol_";
        private const string sSortDirPrefixParameter = "sSortDir_";
        private const string bSortablePrefixParameter = "bSortable_";
        private const string sSearchParameter = "sSearch";
        private const string bRegexParameter = "bRegex";
        private const string bSearchablePrefixParameter = "bSearchable_";
        private const string sSearchPrefixParameter = "sSearch_";
        private const string bEscapeRegexPrefixParameter = "bRegex_";
        private readonly string echo;
        private readonly int displayStart;
        private readonly int displayLength;
        private readonly int sortingCols;
        private readonly SortColumn[] sortColumns;
        private readonly int ColumnCount;
        private readonly Column[] columns;
        private readonly string search;
        private readonly bool regex;
        public string sEcho
        {
            get
            {
                return this.echo;
            }
        }
        public int iDisplayStart
        {
            get
            {
                return this.displayStart;
            }
        }
        public int iDisplayLength
        {
            get
            {
                return this.displayLength;
            }
        }
        public int iSortingCols
        {
            get
            {
                return this.sortingCols;
            }
        }
        public SortColumn[] SortColumns
        {
            get
            {
                return this.sortColumns;
            }
        }
        public string SortString
        {
            get
            {
                string text = string.Empty;
                SortColumn[] array = this.SortColumns;
                for (int i = 0; i < array.Length; i++)
                {
                    SortColumn sortColumn = array[i];
                    text = text + "," + string.Format("{0} {1}", sortColumn.Name, sortColumn.Direction);
                }
                return (text.Length > 0) ? text.Substring(1) : string.Empty;
            }
        }
        public int iColumns
        {
            get
            {
                return this.ColumnCount;
            }
        }
        public Column[] Columns
        {
            get
            {
                return this.columns;
            }
        }
        public string Search
        {
            get
            {
                return this.search;
            }
        }
        public bool Regex
        {
            get
            {
                return this.regex;
            }
        }
        public DataTablesRequest(HttpRequestBase request)
        {
            this.request = request;
            this.echo = this.ParseStringParameter("sEcho");
            this.displayStart = this.ParseIntParameter("iDisplayStart");
            this.displayLength = this.ParseIntParameter("iDisplayLength");
            this.sortingCols = this.ParseIntParameter("iSortingCols");
            this.search = this.ParseStringParameter("sSearch");
            this.regex = (this.ParseStringParameter("bRegex") == "true");
            int num = this.iSortingCols;
            this.sortColumns = new SortColumn[num];
            for (int i = 0; i < num; i++)
            {
                SortColumn sortColumn = new SortColumn();
                sortColumn.Index = this.ParseIntParameter(string.Format("iSortCol_{0}", i));
                sortColumn.Name = this.ParseStringParameter(string.Format("mDataProp_{0}", sortColumn.Index));
                sortColumn.Direction = "ASC";
                bool flag = this.ParseStringParameter(string.Format("sSortDir_{0}", i)).ToUpper() == "DESC";
                if (flag)
                {
                    sortColumn.Direction = "DESC";
                }
                this.sortColumns[i] = sortColumn;
            }
            this.ColumnCount = this.ParseIntParameter("iColumns");
            num = this.ColumnCount;
            this.columns = new Column[num];
            string[] array = this.ParseStringParameter("sColumns").Split(new char[]
            {
                ','
            });
            bool flag2 = array != null && array.Count<string>() > 1;
            if (flag2)
            {
                for (int j = 0; j < num; j++)
                {
                    Column column = new Column();
                    column.Name = array[j];
                    column.Sortable = (this.ParseStringParameter(string.Format("bSortable_{0}", j)) == "true");
                    column.Searchable = (this.ParseStringParameter(string.Format("bSearchable_{0}", j)) == "true");
                    column.Search = this.ParseStringParameter(string.Format("sSearch_{0}", j));
                    column.EscapeRegex = (this.ParseStringParameter(string.Format("bRegex_{0}", j)) == "true");
                    this.columns[j] = column;
                }
            }
        }
        public DataTablesRequest(HttpRequest httpRequest) : this(new HttpRequestWrapper(httpRequest))
        {
        }
        private int ParseIntParameter(string name)
        {
            int result = 0;
            string text = this.request[name];
            bool flag = !string.IsNullOrEmpty(text);
            if (flag)
            {
                int.TryParse(text, out result);
            }
            return result;
        }
        private string ParseStringParameter(string name)
        {
            return this.request[name];
        }
        private bool ParseBooleanParameter(string name)
        {
            bool result = false;
            string value = this.request[name];
            bool flag = !string.IsNullOrEmpty(value);
            if (flag)
            {
                bool.TryParse(value, out result);
            }
            return result;
        }
    }


    public class SortColumn
    {
        public int Index
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Direction
        {
            get;
            set;
        }
    }

    public class Column
    {
        public string Name
        {
            get;
            set;
        }
        public bool Sortable
        {
            get;
            set;
        }
        public bool Searchable
        {
            get;
            set;
        }
        public string Search
        {
            get;
            set;
        }
        public bool EscapeRegex
        {
            get;
            set;
        }
    }
}

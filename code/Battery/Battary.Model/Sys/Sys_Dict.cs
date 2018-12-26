using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
namespace Battery.Model
{
    [Serializable]
    public class Sys_Dict : BasicModel
    {

        /// <summary>
        /// Id
        /// </summary>		
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// GroupCode
        /// </summary>		
        private string _groupcode;
        public string GroupCode
        {
            get { return _groupcode; }
            set { _groupcode = value; }
        }
        /// <summary>
        /// ConfigCode
        /// </summary>		
        private string _configcode;
        public string ConfigCode
        {
            get { return _configcode; }
            set { _configcode = value; }
        }
        /// <summary>
        /// ConfigValue
        /// </summary>		
        private string _configvalue;
        public string ConfigValue
        {
            get { return _configvalue; }
            set { _configvalue = value; }
        }
        /// <summary>
        /// SortNo
        /// </summary>		
        private int _sortno;
        public int SortNo
        {
            get { return _sortno; }
            set { _sortno = value; }
        }
        public int IsSys { get; set; }
    }
}


using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
namespace Battery.Model
{
    //Sys_Log
    [Serializable]
    public class Sys_Log : BasicModel
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
        /// PersonId
        /// </summary>		
        private int _personid;
        public int PersonId
        {
            get { return _personid; }
            set { _personid = value; }
        }
        /// <summary>
        /// Message
        /// </summary>		
        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        /// <summary>
        /// Result
        /// </summary>		
        private string _result;
        public string Result
        {
            get { return _result; }
            set { _result = value; }
        }
        /// <summary>
        /// IPAddress
        /// </summary>		
        private string _ipaddress;
        public string IPAddress
        {
            get { return _ipaddress; }
            set { _ipaddress = value; }
        }
        /// <summary>
        /// Memo
        /// </summary>		
        private string _memo;
        public string Memo
        {
            get { return _memo; }
            set { _memo = value; }
        }
        /// <summary>
        /// CreatePerson
        /// </summary>		
        private int _createperson;
        public int CreatePerson
        {
            get { return _createperson; }
            set { _createperson = value; }
        }
        /// <summary>
        /// CreateTime
        /// </summary>		
        private DateTime _createtime;
        public DateTime CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }

    }
}


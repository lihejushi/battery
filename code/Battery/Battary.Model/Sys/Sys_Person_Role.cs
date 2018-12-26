using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
namespace Battery.Model
{
    //Sys_Person_Role
    [Serializable]
    public class Sys_Person_Role : BasicModel
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
        /// RoleId
        /// </summary>		
        private int _roleid;
        public int RoleId
        {
            get { return _roleid; }
            set { _roleid = value; }
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

    }
}


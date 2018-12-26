using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
namespace Battery.Model
{
    //Sys_Role_Permission
    [Serializable]
    public class Sys_Role_Permission : BasicModel
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
        /// Controller
        /// </summary>		
        private string _controller;
        public string Controller
        {
            get { return _controller; }
            set { _controller = value; }
        }
        /// <summary>
        /// Action
        /// </summary>		
        private string _action;
        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }

    }
}


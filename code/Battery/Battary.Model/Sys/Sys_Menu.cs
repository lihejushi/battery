using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
namespace Battery.Model
{
    [Serializable]
    public class Sys_Menu : BasicModel
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
        /// Name
        /// </summary>		
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Url
        /// </summary>		
        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
        /// <summary>
        /// Icon
        /// </summary>		
        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
        /// <summary>
        /// ViewPermission
        /// </summary>		
        private string _viewpermission;
        public string ViewPermission
        {
            get { return _viewpermission; }
            set { _viewpermission = value; }
        }
        /// <summary>
        /// ParentId
        /// </summary>		
        private int _parentid;
        public int ParentId
        {
            get { return _parentid; }
            set { _parentid = value; }
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
        /// State
        /// </summary>		
        private int _state;
        public int State
        {
            get { return _state; }
            set { _state = value; }
        }
        
        public virtual List<Sys_Menu> ChildMenus { get; set; }
    }
}


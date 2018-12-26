using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace XT.MVC.Framework
{
    public partial class BaseXTModel
    {
        public BaseXTModel()
        {
            this.CustomProperties = new Dictionary<string, object>();
        }
        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }

        public Dictionary<string, object> CustomProperties { get; set; }
    }
}

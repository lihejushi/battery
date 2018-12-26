using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Battery.Areas.SysManager.Validators;
using Battery.Model;

namespace Battery.Areas.SysManager.Models
{
    [Validator(typeof(SysPersonModelValidator))]
    public class SysPersonModel : Sys_Person
    {
    }
}
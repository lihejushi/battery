using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Battery.Areas.SysManager.Validators;
using Battery.Model;
using Battery.Model.Battery;

namespace Battery.Areas.SysManager.Models
{
    [Validator(typeof(ProductTypeModelValidator))]
    public class ProductTypeModel: ProductType
    {
    }
}
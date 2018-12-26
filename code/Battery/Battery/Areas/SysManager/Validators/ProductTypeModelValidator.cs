using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Battery.Areas.SysManager.Models;

namespace Battery.Areas.SysManager.Validators
{
    public class ProductTypeModelValidator: AbstractValidator<ProductTypeModel>
    {
        public ProductTypeModelValidator()
        {
            RuleFor(x => x.TypeName).NotEmpty().WithMessage("请填写商品类型名称").Length(1, 50).WithMessage("商品类型名称长度为1-50字符");
            RuleFor(x => x.TypeCode).NotEmpty().WithMessage("请填写商品类型编码").Length(6, 16).WithMessage("编码长度为1-50字符");
            RuleFor(x => x.Length).NotEmpty().WithMessage("请填写长度");
            RuleFor(x => x.Width).NotEmpty().WithMessage("请填写宽度");
            RuleFor(x => x.Weight).NotEmpty().WithMessage("请填写重量");
        }
    }
}
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Battery.Areas.SysManager.Models;

namespace Battery.Areas.SysManager.Validators
{
    public class SysPersonModelValidator: AbstractValidator<SysPersonModel>
    {
        public SysPersonModelValidator()
        {
          
            RuleFor(x => x.UserName).NotEmpty().WithMessage("请填写用户名").Length(6, 16).WithMessage("用户名长度为6-16字符");
            RuleFor(x => x.Password).Length(6, 16).WithMessage("密码长度为6-16字符");
            RuleFor(x => x.TrueName).NotEmpty().WithMessage("请填写真实姓名").Length(1, 32).WithMessage("真实姓名长度为1-32字符");
            RuleFor(x => x.Address).Length(1, 128).WithMessage("地址长度过长");
            //RuleFor(x => x.MobilePhone).Length(1, 32).WithMessage("电话格式不正确");
            //RuleFor(x => x.Tel).Length(1, 32).WithMessage("电话格式不正确");
            RuleFor(x => x.Email).EmailAddress().WithMessage("电子邮箱格式不正确");
            RuleFor(x => x.MobilePhone).NotEmpty().WithMessage("请填写联系电话").Length(1, 32).WithMessage("企业联系电话格式不正确");
            RuleFor(x => x.Memo).Length(1, 512).WithMessage("备注最长512位字符");
        }
    }
}
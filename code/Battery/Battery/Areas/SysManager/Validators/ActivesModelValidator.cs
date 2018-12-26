using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Battery.Areas.SysManager.Models;
namespace Battery.Areas.SysManager.Validators
{
    public class ActivesModelValidator : AbstractValidator<ActivesModel>
    {
        public ActivesModelValidator()
        {
            RuleFor(x => x.ActiveName).NotEmpty().WithMessage("请填写活动名称").Length(1, 64).WithMessage("资源名称长度为1-64个字符");
            RuleFor(x => x.ContributeStartTimeT).NotEmpty().WithMessage("请填写投稿开始时间");
            RuleFor(x => x.ContributeEndTimeT).NotEmpty().WithMessage("请填写投稿结束时间");
            //RuleFor(x => x.ValidTime).NotEmpty().WithMessage("请填写验证有效期");
            RuleFor(x => x.MaxDocument).NotEmpty().WithMessage("请填写单用户文档上限");
            RuleFor(x => x.MaxDocument).GreaterThanOrEqualTo(0).WithMessage("请正确填写用户文档上限（大于0的整数）");
            RuleFor(x => x.MaxPic).NotEmpty().WithMessage("请填写单用户照片上限");
            RuleFor(x => x.MaxPic).GreaterThanOrEqualTo(0).WithMessage("请正确填写用户照片上限（大于0的整数）");
            RuleFor(x => x.Summary).NotEmpty().WithMessage("请填写活动简介");
        }

    }
}
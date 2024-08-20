using FluentValidation;
using WajeSmartAssessment.Application.Dtos;
using WajeSmartAssessment.Application.Features.Engagement.Commands;

namespace WajeSmartAssessment.Application.Features.Engagement.Validators;
public class LikeOrUnlikeCommandValidator : AbstractValidator<LikePostCommand>
{
    public LikeOrUnlikeCommandValidator()
    {
        RuleFor(x => x.PostId).ValidGuid();
    }
}

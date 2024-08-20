using FluentValidation;
using WajeSmartAssessment.Application.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Queries;

namespace WajeSmartAssessment.Application.Features.Posts.Validators;
public class GetPostByIdQueryValidator : AbstractValidator<GetPostByIdQuery>
{
    public GetPostByIdQueryValidator()
    {
        RuleFor(x => x.PostId).ValidGuid();
    }
}

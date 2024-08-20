using FluentValidation;
using WajeSmartAssessment.Application.Dtos;
using WajeSmartAssessment.Application.Features.Blogs.Queries;

namespace WajeSmartAssessment.Application.Features.Blogs.Validations;
public class GetBlogByIdQueryValidator : AbstractValidator<GetBlogByIdQuery>
{
    public GetBlogByIdQueryValidator()
    {
        RuleFor(x => x.BlogId).ValidGuid();
    }
}

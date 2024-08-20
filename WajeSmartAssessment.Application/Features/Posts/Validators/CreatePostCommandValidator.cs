using FluentValidation;
using WajeSmartAssessment.Application.Dtos;
using WajeSmartAssessment.Application.Features.Posts.Commands;

namespace WajeSmartAssessment.Application.Features.Posts.Validators;
public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required.");

        RuleFor(x => x.BlogId).NotEmpty().WithMessage("BlogId is required").ValidGuid();
    }
}

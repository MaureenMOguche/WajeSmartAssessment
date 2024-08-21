using FluentValidation;
using WajeSmartAssessment.Application.Dtos;
using WajeSmartAssessment.Application.Features.Comments.Commands;

namespace WajeSmartAssessment.Application.Features.Comments.Validations;
public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.PostId).ValidGuid();
        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required");
    }

}

using FluentValidation;

namespace WajeSmartAssessment.Application.Dtos;
public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> ValidGuid<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(BeAValidGuid).WithMessage("Invalid GUID format.");
    }

    private static bool BeAValidGuid(string guidString)
    {
        return Guid.TryParse(guidString, out _);
    }
}
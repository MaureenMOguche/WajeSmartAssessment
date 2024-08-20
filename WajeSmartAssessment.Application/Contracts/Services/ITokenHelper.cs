using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Contracts.Services;
public interface ITokenHelper
{
    public string GenerateLoginToken(AppUser user);
}

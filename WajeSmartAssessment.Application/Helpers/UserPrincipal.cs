namespace WajeSmartAssessment.Application.Helpers;

public class UserPrincipal
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public bool IsAdmin => Role == "Admin";
    
}

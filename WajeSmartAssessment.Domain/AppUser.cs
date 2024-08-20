using Microsoft.AspNetCore.Identity;

namespace WajeSmartAssessment.Domain;
public class AppUser : IdentityUser
{
    protected AppUser(): base() { }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateTime RefreshExpiry { get; private set; }
    public string RefreshToken { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; set; } = true;


    private AppUser(string firstName, string lastname, string username, 
        string email, UserRole role, string? avatarUrl = null)
    {
        Id = Guid.NewGuid().ToString();
        FirstName = firstName;
        LastName = lastname;
        UserName = username;
        Email = email;
        AvatarUrl = avatarUrl;
        Role = role;
    }

    public string FullName => $"{FirstName} {LastName}";

    public static AppUser Create(
        string firstName, 
        string lastname, 
        string username, 
        string email, 
        string? avatarUrl = null)
        => new(firstName, lastname, username, email, UserRole.Author, avatarUrl);


    public static AppUser CreateAdmin(
        string firstName,
        string lastname,
        string username,
        string email,
        string? avatarUrl = null)
        => new(firstName, lastname, username, email, UserRole.Admin, avatarUrl);


}

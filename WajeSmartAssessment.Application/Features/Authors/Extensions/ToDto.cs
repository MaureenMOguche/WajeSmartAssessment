using WajeSmartAssessment.Application.Features.Authors.Dtos;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Application.Features.Authors.Extensions;
public static class AuthorExtensions
{
    public static AuthorInfoDto ToDto(this AppUser author, int noOfPosts)
    {
        return new AuthorInfoDto
        {
            Id = author.Id,
            Name = $"{author.FirstName} {author.LastName}",
            Email = author.Email!,
            NoOfPosts = noOfPosts,
            AvatarUrl = author.AvatarUrl
        };
    }

    public static AuthorDto ToDto(this AppUser author)
    {
        return new AuthorDto
        {
            Id = author.Id,
            Name = $"{author.FirstName} {author.LastName}",
            Email = author.Email!,
            AvatarUrl = author.AvatarUrl
        };
    }
}

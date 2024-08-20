using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WajeSmartAssessment.Application.Contracts.Repository;
using WajeSmartAssessment.Domain;
using WajeSmartAssessment.Infrastructure.Repositories;

namespace WajeSmartAssessment.Infrastructure;
public static class RegisterPersistence
{
    public static void AddPersistenceServices(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
}

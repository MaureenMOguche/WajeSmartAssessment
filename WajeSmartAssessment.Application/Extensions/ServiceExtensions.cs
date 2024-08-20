using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using WajeSmartAssessment.Application.Middlewares;
using Serilog;
using WajeSmartAssessment.Application.Config;
using Asp.Versioning;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ReenUtility.Services;
using WajeSmartAssessment.Application.Contracts.Services;
using WajeSmartAssessment.Application.Implementations;
using WajeSmartAssessment.Application.Features.Blogs.Commands;
using WajeSmartAssessment.Application.Contracts.Repository;
using Hangfire;
using WajeSmartAssessment.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Security.Claims;
using WajeSmartAssessment.Application.Features.Blogs.Validations;

namespace WajeSmartAssessment.Application.Extensions;
public static class ServiceExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        SetupSwagger(services);
        ConfigureSerilog(builder);
        SetupAuthentication(services, builder.Configuration);
        BindConfigurations(services);
        AddApiVersioning(services);
        RegisterFilters(services);
        AddServiceDependencies(services);
        RegisterFluentValidation(services);
        ConfigureHangfire(services, builder.Configuration);
        SetupControllers(services);
    }

    private static void ConfigureHangfire(IServiceCollection services, IConfiguration config)
    {
        services.AddHangfire(cfg =>
        {
            cfg.UseSqlServerStorage(config.GetConnectionString("DefaultConnection"));
        });

        services.AddHangfireServer();
    }

    private static void AddServiceDependencies(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddScoped<IHttpService, HttpService>();
        services.AddScoped<IFirebaseService, FirebaseService>();
        services.AddSingleton<ITokenHelper, TokenHelper>();
        services.AddValidatorsFromAssemblyContaining<GetBlogByIdQueryValidator>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetCallingAssembly());
            cfg.RegisterServicesFromAssemblyContaining<CreateBlogCommand>();
        });
    }

    private static void SetupAuthentication(IServiceCollection services, IConfiguration config)
    {
        var jwtSettings = config.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(cfg =>
        {

            
            cfg.TokenValidationParameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = false,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
            };

            cfg.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    // Override the default behavior of redirecting to a login page
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize("You are not authorized");
                    return context.Response.WriteAsync(result);
                }
            };
        });



        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
           policy.RequireClaim(ClaimTypes.Role, "Admin"));
        });
    }

    private static void RegisterFilters(IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalErrorHandling>();
        services.AddProblemDetails();
    }

    private static void SetupSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlfile = "WajeSmartAssessment.Api.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlfile);
            options.IncludeXmlComments(xmlPath);

            options.SwaggerDoc("v1", new OpenApiInfo { Title = "WajeSmartAssessment", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    private static void RegisterFluentValidation(IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssemblyContaining<GetBlogByIdQueryValidator>();
    }

    private static void AddApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;

            var apiReader = new UrlSegmentApiVersionReader();
            options.ApiVersionReader = apiReader;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    private static void BindConfigurations(IServiceCollection services)
    {
        services.AddOptions<JwtSettings>()
            .BindConfiguration(nameof(JwtSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    private static void ConfigureSerilog(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    private static void SetupControllers(IServiceCollection services)
    {
        services.AddControllers();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = ModelValidation.ValidateModel;
        });
    }
}

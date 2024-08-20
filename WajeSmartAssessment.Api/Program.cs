using WajeSmartAssessment.Application.Extensions;
using WajeSmartAssessment.Application.Helpers;
using WajeSmartAssessment.Infrastructure;
using WajeSmartAssessment.Infrastructure.SeedData;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterApplicationServices(builder);
builder.Services.AddPersistenceServices();

_ = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var app = builder.Build();
app.ConfigureApplication();
var scopeFactory = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>();
var contextAccessor = ((IApplicationBuilder)app).ApplicationServices.GetService<IHttpContextAccessor>();

if (contextAccessor != null)
    UserHelper.Configure(contextAccessor);

if (scopeFactory != null)
{
    using var scope = scopeFactory.CreateScope();
    var provider = scope.ServiceProvider;
    await WajeSmartSeedData.InitializeAsync(provider);
}

app.Run();

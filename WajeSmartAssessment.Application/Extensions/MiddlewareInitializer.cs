using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace WajeSmartAssessment.Application.Extensions;
public static class MiddlewareInitializer
{
    public static void ConfigureApplication(this WebApplication app)
    {
        RegisterSwagger(app);
        RegisterMiddlewares(app);
    }

    private static void RegisterSwagger(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    private static void RegisterMiddlewares(WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors(opt =>
        {
            opt.AllowAnyOrigin();
            opt.AllowAnyHeader();
            opt.AllowAnyMethod();
        });


        //app.UseStaticFiles();

        app.MapGet("/", async (context) => await context.Response
        .WriteAsync(PageTemplates.PageTemplates.GetIndexPage(Assembly
        .GetExecutingAssembly().GetName().Name, "relogosquare.jpg")));


        app.Use(async (context, next) =>
        {
            Console.WriteLine("About to hit authentication middleware");
            await next.Invoke();
            Console.WriteLine("Passed authentication middleware");
        });

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseExceptionHandler();


        app.MapControllers();
    }
}

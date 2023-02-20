using Microsoft.OpenApi.Models;

namespace IdentityWebApi.Extensions;

public static class SwaggerSetup
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Identity Web Api",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Henrique Aguiar"
                }
            });
        });
    }

    public static void UseSwaggerUI(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("v1/swagger.json", "Api interna");
            options.RoutePrefix = "swagger";
        });
    }
}
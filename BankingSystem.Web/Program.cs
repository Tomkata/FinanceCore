using BankingSystem.Application;
using BankingSystem.Application.Common.Mappings;
using BankingSystem.Domain.DomainServices;
using BankingSystem.Infrastructure;
using BankingSystem.Infrastructure.Data;
using BankingSystem.Web.Middleware;
using Microsoft.AspNetCore.Diagnostics;

public class Program
{
    private static async Task Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

        MappingConfig.RegisterMappings();

        builder.Services
            .AddApplication()
            .AddInfrastructure(builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Banking System API",
                Version = "v1",
                Description = "A banking system API with JWT authentication"
            });

            // Add JWT Authentication
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below."
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        var app = builder.Build();

        //using (var scope = app.Services.CreateScope())
        //{
        //    var services = scope.ServiceProvider;
        //    var context = services.GetRequiredService<ApplicationDbContext>();
        //    var configuration = services.GetRequiredService<IConfiguration>();

        //    await DatabaseSeeder.SeedAsync(context, configuration);
        //}

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
       
        app.Run();
    }
}
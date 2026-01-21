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
        builder.Services.AddSwaggerGen();

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
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
       
        app.Run();
    }
}
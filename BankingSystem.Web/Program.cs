using BankingSystem.Application;
using BankingSystem.Application.Common.Mappings;
using BankingSystem.Infrastructure;
using BankingSystem.Web.Middleware;
using Microsoft.AspNetCore.Diagnostics;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        MappingConfig.RegisterMappings();

        // Add services
        builder.Services
            .AddApplication()
            .AddInfrastructure(builder.Configuration);

        builder.Services.AddControllers();  
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

      

        // Configure pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();  

        app.Run();
    }
}
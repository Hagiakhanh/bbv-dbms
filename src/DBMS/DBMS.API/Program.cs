using DBMS.Domain;
using DBMS.Application;
using DBMS.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace DBMS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Đăng ký Dependency Injection từ các tầng Clean Architecture
            builder.Services
               .AddDomainServices()             // DBMS.Domain
               .AddApplicationServices()        // DBMS.Application
               .AddInfrastructureServices();    // DBMS.Infrastructure

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger"));

            app.Run();
        }
    }
}

using DBMS.API.Repositories.Databases;
using DBMS.API.Repositories.Schemas;
using DBMS.API.Services.Databases;
using DBMS.API.Services.Schemas;
using DBMS.Domain;
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

            // Đăng ký các Domain Services (lõi DBMS)
            builder.Services.AddDomainServices();

            // Đăng ký Database Repositories và Services
            builder.Services.AddSingleton<IDatabaseRepository, InMemoryDatabaseRepository>();
            builder.Services.AddScoped<IDatabaseService, DatabaseService>();

            // Đăng ký Schema Repositories và Services
            builder.Services.AddSingleton<ISchemaRepository, InMemorySchemaRepository>();
            builder.Services.AddScoped<ISchemaService, SchemaService>();

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
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

            app.Run();
        }
    }
}

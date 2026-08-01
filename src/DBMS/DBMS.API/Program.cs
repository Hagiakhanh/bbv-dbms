using DBMS.API.Repositories.Columns;
using DBMS.API.Repositories.Constraints;
using DBMS.API.Repositories.Databases;
using DBMS.API.Repositories.Indexes;
using DBMS.API.Repositories.Schemas;
using DBMS.API.Repositories.Tables;
using DBMS.API.Services.Columns;
using DBMS.API.Services.Constraints;
using DBMS.API.Services.Databases;
using DBMS.API.Services.Indexes;
using DBMS.API.Services.Schemas;
using DBMS.API.Services.Tables;
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

            // Đăng ký Table Repositories và Services
            builder.Services.AddSingleton<ITableRepository, InMemoryTableRepository>();
            builder.Services.AddScoped<ITableService, TableService>();

            // Đăng ký Column Repositories và Services
            builder.Services.AddSingleton<IColumnRepository, InMemoryColumnRepository>();
            builder.Services.AddScoped<IColumnService, ColumnService>();

            // Đăng ký Constraint Repositories và Services
            builder.Services.AddSingleton<IConstraintRepository, InMemoryConstraintRepository>();
            builder.Services.AddScoped<IConstraintService, ConstraintService>();

            // Đăng ký Index Repositories và Services
            builder.Services.AddSingleton<IIndexRepository, InMemoryIndexRepository>();
            builder.Services.AddScoped<IIndexService, IndexService>();


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

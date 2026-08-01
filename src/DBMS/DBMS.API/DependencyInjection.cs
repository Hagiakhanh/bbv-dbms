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
using Microsoft.Extensions.DependencyInjection;

namespace DBMS.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            // Đăng ký các Domain Services (lõi DBMS)
            services.AddDomainServices();

            // Đăng ký Database Repositories và Services
            services.AddSingleton<IDatabaseRepository, InMemoryDatabaseRepository>();
            services.AddScoped<IDatabaseService, DatabaseService>();

            // Đăng ký Schema Repositories và Services
            services.AddSingleton<ISchemaRepository, InMemorySchemaRepository>();
            services.AddScoped<ISchemaService, SchemaService>();

            // Đăng ký Table Repositories và Services
            services.AddSingleton<ITableRepository, InMemoryTableRepository>();
            services.AddScoped<ITableService, TableService>();

            // Đăng ký Column Repositories và Services
            services.AddSingleton<IColumnRepository, InMemoryColumnRepository>();
            services.AddScoped<IColumnService, ColumnService>();

            // Đăng ký Constraint Repositories và Services
            services.AddSingleton<IConstraintRepository, InMemoryConstraintRepository>();
            services.AddScoped<IConstraintService, ConstraintService>();

            // Đăng ký Index Repositories và Services
            services.AddSingleton<IIndexRepository, InMemoryIndexRepository>();
            services.AddScoped<IIndexService, IndexService>();

            return services;
        }
    }
}

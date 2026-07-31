using DBMS.Domain.Interfaces;
using DBMS.Infrastructure.Persistence;
using DBMS.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DBMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services)
        {
            services.AddRepositories();

            return services;
        }

        // ── Repositories ─────────────────────────────────────────────────
        private static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            services.AddSingleton<IDatabaseRepository, InMemoryDatabaseRepository>();
            return services;
        }

    }
}

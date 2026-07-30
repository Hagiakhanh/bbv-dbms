using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainServices(
           this IServiceCollection services)

        {
            
            return services;
        }
    }
}

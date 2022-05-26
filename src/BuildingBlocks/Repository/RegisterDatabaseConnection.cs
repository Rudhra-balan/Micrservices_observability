using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Repository
{
    public static class RegisterDatabaseConnection
    {
        public static void AddDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConnection>(options => configuration.GetSection("DBOption").Bind(options));
        }
    }
}

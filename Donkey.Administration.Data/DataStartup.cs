using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Donkey.Administration.Data
{
    public static class DataStartup
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddDbContextFactory<DataContext>((s, options) =>
            {
                // Пример строки подключения
                //Host=127.0.0.1;Database=Donkey.Administration;Username=postgres;Password=password;

                var config = s.GetRequiredService<IConfiguration>();
                
                options.UseNpgsql(config["ConnectionString"]);

            });
            services.AddSingleton<IClientDataAccess, ClientDataAccess>();
            services.AddSingleton<IOrderDataAccess, OrderDataAccess>();
            services.AddSingleton<IAdminDataAccess, AdminDataAccess>();
        }
    }
}

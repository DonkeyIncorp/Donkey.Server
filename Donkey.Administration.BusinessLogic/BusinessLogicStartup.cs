using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donkey.Administration.BusinessLogic
{
    public static class BusinessLogicStartup
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IClientManager, ClientManager>();
            services.AddSingleton<IOrderManager, OrderManager>();

            services.AddHostedService<EmailSender>();
        }
    }
}

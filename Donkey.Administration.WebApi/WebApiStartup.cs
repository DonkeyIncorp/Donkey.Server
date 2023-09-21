using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text;
using Swashbuckle.AspNetCore;
using System.Reflection;
using System.IO;

namespace Donkey.Administration.WebApi
{
    public static class WebApiStartup
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IGeneralKeyManager, GeneralKeyManager>();
            services.AddSingleton<IAdminKeyManager, AdminsKeyManager>();
            services.AddSingleton<IHashEncoder, Sha265Encoder>();

            services.AddMvcCore().AddControllersAsServices();



            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Система API (c) Donkey.inc",
                    Description = "Описание всех доступных методов API системы. Для информации о получении ключей свяжитесь с главным Backend разработчиком"
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });



            services.AddCors(options =>
            {
                options.AddPolicy("cors", builder =>
                {
                    builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
        }
    }
}

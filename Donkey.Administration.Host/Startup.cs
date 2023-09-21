using Donkey.Administration.BusinessLogic;
using Donkey.Administration.Data;
using Donkey.Administration.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Donkey.Administration.Host
{
    public class Startup
    {
        //Donkey.Administration
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            DataStartup.RegisterServices(services);
            BusinessLogicStartup.RegisterServices(services);
            WebApiStartup.RegisterServices(services);

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "swagger demo api");
            });
        }
    }
}

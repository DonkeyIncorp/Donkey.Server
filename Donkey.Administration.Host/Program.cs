using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Donkey.Administration.Host
{
    // запуск ngrok: ngrok http 1258 -host-header="localhost:1258"
    public class Program
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication();

            var consoleOption = app.Option("--console", "Запуск в режиме консоли", CommandOptionType.NoValue);
            var configPathOption = app.Option("--config", "Запуск с параметрами из файла конфигурации", CommandOptionType.SingleValue);


            app.OnExecute(() =>
            {
                if (configPathOption.HasValue())
                {
                    IConfiguration configuration = new ConfigurationBuilder().AddJsonFile(configPathOption.Value()).Build();

                    var host = CreateWebHostBuilder(configuration).Build();
                    
                    if (consoleOption.HasValue())
                    {
                        host.Run();
                    }
                    else
                    {
                        string pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                        var rootPath = Path.GetDirectoryName(pathToExe);

                        Directory.SetCurrentDirectory(rootPath);

                        host.RunAsService();
                    }
                }
                else
                {
                    throw new Exception("No config path entered");
                }

                return 0;
            });

            try
            {
                app.Execute(args);
            }
            catch (OperationCanceledException)
            {

            }
            catch(Exception ex)
            {
                Logger.Fatal(ex);
            }
            finally
            {
                NLog.LogManager.Flush();

                Console.ReadKey();
            }
            
        }


        public static IWebHostBuilder CreateWebHostBuilder(IConfiguration config) =>
            new WebHostBuilder()
            .UseKestrel()
            .UseUrls(GetStartUrl(config))
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .UseStartup<Startup>()
            .UseConfiguration(config);


        private static string GetStartUrl(IConfiguration config) => $"{config["Host"]}:{config["Port"]}";
        
    }
}

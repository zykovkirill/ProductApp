using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductApp.Server.Services;
using ProductApp.Shared.Models;
using Serilog;
using System;

namespace ProductApp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}";
                Log.Logger = new
                    LoggerConfiguration().WriteTo.File(AppDomain.CurrentDomain.BaseDirectory + "\\Log.txt",
                    rollingInterval: RollingInterval.Day, outputTemplate:
                    outputTemplate).CreateLogger();
                var host = CreateHostBuilder(args).Build();
                host.Run();
                //TODO: CancellationToken
                // host.RunAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseKestrel();
                }).UseSerilog();
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
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
                CreateHostBuilder(args).Build().Run();
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

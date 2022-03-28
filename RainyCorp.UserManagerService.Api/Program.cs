using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RainyCorp.UserManagerService.Shared.Settings;
using Serilog;
using System;
using System.Threading.Tasks;

namespace RainyCorp.UserManagerService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.InitAsync();
            host.Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                     .AddJsonFile("appsettings.json", false)
                      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                     .Build();

            Log.Logger = new LoggerConfiguration()
                       .ReadFrom.Configuration(config)
                       .CreateLogger();
            var protocols = new WebProtocolSettings();
            config.GetSection("WebProtocolSettings").Bind(protocols);
            return WebHost.CreateDefaultBuilder(args)
                     .UseSerilog()
                     .UseUrls(protocols.Urls)
                     .UseStartup<Startup>();
        }
    }
}

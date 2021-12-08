using System;
using System.IO;
using AudioBoos.Server.Services.Startup.SSL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace AudioBoos.Server;

public class Program {
    public static void Main(string[] args) {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
        
        CreateHostBuilder(args, config).Build().Run();
    }

    static IHostBuilder CreateHostBuilder(string[] args, IConfigurationRoot config) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder
                    .UseWebRoot(config.GetSection("System").GetValue<string>("CachePath"))
                    .UseStartup<Startup>()
                    .UseKestrel(options => options.ConfigureEndpoints());
            })
            .ConfigureLogging(builder => {
                builder.ClearProviders();
                builder.AddConsole();
            });
}

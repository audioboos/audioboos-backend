using System;
using System.IO;
using System.Reflection;
using AudioBoos.Server.Services.Startup.SSL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace AudioBoos.Server;

public class Program {
#if DEBUG
    private const string MONIKER = "DEBUG";
#else
const string MONIKER = "PROD";
#endif
    public static string GetAssemblyDetails() {
        var assembly = typeof(Program).Assembly;
        return
            $"\tVersion: {assembly.GetName()?.Version?.ToString()}\n" +
            $"\tFileVersion: {System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion}\n" +
            $"\tInformationalVersion: {assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}\n";
    }

    public static void Main(string[] args) {
        Console.WriteLine($"Bootstrapping AudioBoos {MONIKER}");
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

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

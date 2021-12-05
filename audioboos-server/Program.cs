using AudioBoos.Server.Services.Startup.SSL;
using Microsoft.AspNetCore.Hosting;
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
        CreateHostBuilder(args).Build().Run();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder
                    .UseStartup<Startup>()
                    .UseKestrel(options => options.ConfigureEndpoints());
            })
            .ConfigureLogging(builder => {
                builder.ClearProviders();
                builder.AddConsole();
            });
}

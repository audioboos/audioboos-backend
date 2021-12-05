using AudioBoos.Server.Services.Startup.SSL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Server;

public class Program {
    public static void Main(string[] args) {
        CreateHostBuilder(args).Build().Run();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
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

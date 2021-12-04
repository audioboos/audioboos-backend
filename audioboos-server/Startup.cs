using System;
using AudioBoos.Data.Access;
using AudioBoos.Data;
using AudioBoos.Data.Interfaces;
using AudioBoos.Data.Store;
using AudioBoos.Server.Services.Startup;
using AudioBoos.Server.Services.Email;
using AudioBoos.Server.Services.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AudioBoos.Server;

public class Startup {
    public IConfiguration Configuration { get; }
    private IWebHostEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment env) {
        Configuration = configuration;
        Environment = env;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
        if (services == null) {
            throw new NullReferenceException("Startup.services cannot be null");
        }

        //TODO: refactor this into separate shared project
        var provider = Configuration.GetValue("Provider", "postgres");

        if (Environment.IsDevelopment() || true) {
            Console.WriteLine("******************************************");
            Console.WriteLine(
                $"Env: {System.Environment.GetEnvironmentVariable("ASPNETCORE_ConnectionStrings__PostgresConnection")}");
            Console.WriteLine(Configuration.GetConnectionString("PostgresConnection"));
            Console.WriteLine("******************************************");
        }

        services.AddDbContext<AudioBoosContext>(
            options => _ = provider switch {
                "postgres" => options.UseNpgsql(
                    Configuration.GetConnectionString("PostgresConnection"),
                    x => x.MigrationsAssembly("AudioBoos.Data")
                ),
                _ => throw new Exception($"Unsupported provider: {provider}")
            });

        services.AddAudioBoosOptions(Configuration)
            .AddAudioBoosJobs(Configuration)
            .AddAudioBoosCors(Configuration)
            .AddAudioBoosIdentity(Configuration);

        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IRepository<TrackPlayLog>, AudioPlayAudioRepository>();
        services.AddTransient<IAudioRepository<AudioFile>, AudioFileAudioRepository>();
        services.AddTransient<IAudioRepository<Artist>, ArtistAudioRepository>();
        services.AddTransient<IAudioRepository<Album>, AlbumAudioRepository>();
        services.AddTransient<IAudioRepository<Track>, TrackRepository>();

        services.AddTransient<IEmailSender, EmailSender>();

        services.AddAudioBoosHttpClients();
        services.AddMapping(Configuration);

        services.AddSignalR();

        // services
        //     .AddImageSharp()
        //     .Configure<PhysicalFileSystemCacheOptions>(options => {
        //         options.CacheFolder = "different-cache";
        //     });

        services.AddControllers();
        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "arse", Version = "v1"});
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<AppUser> userManager) {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AudioBoos Server v1"));
        } else {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }


        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAudioBoosCors()
            .UseAudioBoosIdentity();

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
            endpoints.MapHub<JobHub>("/hubs/job");
        });


        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetService<AudioBoosContext>();

        Console.WriteLine("Migrating database");
        context?.Database.Migrate();

        Console.WriteLine("Seeding database");
        AudioBoosDbInitializer.SeedUsers(userManager);
    }
}

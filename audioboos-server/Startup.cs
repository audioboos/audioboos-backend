using System;
using System.IO;
using System.Text.Json.Serialization;
using AudioBoos.Data.Access;
using AudioBoos.Data;
using AudioBoos.Data.Interfaces;
using AudioBoos.Data.Store;
using AudioBoos.Server.Services.Startup;
using AudioBoos.Server.Services.Email;
using AudioBoos.Server.Services.Hubs;
using EntityFrameworkCore.Triggered;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Microsoft.OpenApi.Models;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Processors;

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

        services.AddDbContext<AudioBoosContext>(
                options => _ = provider switch {
                    "postgres" => options.UseNpgsql(
                        Configuration.GetConnectionString("PostgresConnection"),
                        x => x.MigrationsAssembly("AudioBoos.Data")
                            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                    ).UseTriggers(),
                    _ => throw new Exception($"Unsupported provider: {provider}")
                })
            .AddTransient<IBeforeSaveTrigger<Artist>, AudioBoos.Data.Triggers.RetainArtistImageData>()
            .AddTransient<IBeforeSaveTrigger<Album>, AudioBoos.Data.Triggers.RetainAlbumImageData>();

        services.AddAudioBoosOptions(Configuration)
            .AddAudioBoosJobs(Configuration)
            .AddAudioBoosCors(Configuration, Environment.IsDevelopment())
            .AddAudioBoosIdentity(Configuration, Environment.IsDevelopment());

        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IRepository<TrackPlayLog>, AudioPlayAudioRepository>();
        services.AddTransient<IAudioRepository<AudioFile>, AudioFileAudioRepository>();
        services.AddTransient<IAudioRepository<Artist>, ArtistAudioRepository>();
        services.AddTransient<IAudioRepository<Album>, AlbumAudioRepository>();
        services.AddTransient<IAudioRepository<Track>, TrackRepository>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddTransient<IEmailSender, EmailSender>();

        services.AddAudioBoosHttpClients(Configuration);

        services.AddMapping(Configuration);

        services.AddSignalR();

        services.AddImageSharp(
                options => {
                    options.MemoryStreamManager = new RecyclableMemoryStreamManager();
                    options.BrowserMaxAge = TimeSpan.FromDays(7);
                    options.CacheMaxAge = TimeSpan.FromDays(365);
                    options.CachedNameLength = 8;
                }).SetRequestParser<QueryCollectionRequestParser>()
            .Configure<PhysicalFileSystemCacheOptions>(options => {
                options.CacheFolder = Path.Combine(Configuration.GetSection("System").GetValue<string>("CachePath"),
                    ".img-cache");
            }).AddProcessor<ResizeWebProcessor>();


        services
            .AddControllers()
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        ;
        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "AudioBoos API v1.0", Version = "v1"});
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<AppUser> userManager,
        ILogger<Startup> logger) {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AudioBoos API v1"));
        } else {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseImageSharp();
        app.UseStaticFiles();

        // app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAudioBoosCors()
            .UseAudioBoosIdentity();

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
            endpoints.MapHub<JobHub>("/hubs/job");
        });


        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetService<AudioBoosContext>();

        logger.LogInformation("Migrating database");
        context?.Database.Migrate();

        logger.LogInformation("Seeding database");
        AudioBoosDbInitializer.SeedUsers(userManager);
        logger.LogInformation("Database seeded");
    }
}

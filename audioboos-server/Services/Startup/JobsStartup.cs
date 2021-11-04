using AudioBoos.Server.Services.Jobs;
using AudioBoos.Server.Services.Jobs.Scanners;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace AudioBoos.Server.Services.Startup;

public static class JobsStartup {
    public static IServiceCollection AddAudioBoosJobs(this IServiceCollection services, IConfiguration config) {
        services.AddQuartz(q => {
            q.SchedulerId = "AudioBoos-Server-Core";
            q.SchedulerName = "AudioBoos Scheduler";

            q.UseMicrosoftDependencyInjectionJobFactory();

            q.AddJob<CacheImagesJob>(opts => opts.WithIdentity(new JobKey("CacheImages")).StoreDurably());
            q.AddJob<UpdateLibraryJob>(opts => opts.WithIdentity(new JobKey("UpdateLibrary")).StoreDurably())
                .AddTrigger(opts => opts
                    .ForJob(new JobKey("UpdateLibrary"))
                    .WithIdentity("UpdateLibrary-trigger")
                    .WithDailyTimeIntervalSchedule(x => x.WithInterval(24, IntervalUnit.Hour)));

            var scanArtistsJobKey = new JobKey("ScanArtists");
            q.AddJob<ScanArtistsJob>(opts => opts.WithIdentity(scanArtistsJobKey).StoreDurably());
        });

        services.AddQuartzServer(options => {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

        services.AddTransient<ILibraryScanner, FastFilesystemLibraryScanner>();
        return services;
    }

    public static IApplicationBuilder UseAudioBoosJobs(this IApplicationBuilder app) {
        return app;
    }
}

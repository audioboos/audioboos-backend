using System.IO;
using AudioBoos.Data.Access;
using AudioBoos.Data.Persistence;
using AudioBoos.Data.Persistence.Interfaces;
using AudioBoos.Data.Store;
using AudioBoos.Server.Services.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace AudioBoos.Server.Tests;

public class DbFixture {
    private static readonly object _lock = new();
    private static bool _databaseInitialized;
    public ServiceProvider ServiceProvider { get; }

    public DbFixture() {
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                path: "appsettings.json",
                optional: false,
                reloadOnChange: true)
            .Build();
        // serviceCollection
        //     .AddDbContext<AudioBoosContext>(
        //         options => options.UseSqlite(
        //             _configuration.GetConnectionString("SQLiteDefaultConnection"),
        //         ServiceLifetime.Transient);        

        serviceCollection
            .AddDbContext<AudioBoosContext>(
                options => options.UseNpgsql(
                    configuration.GetConnectionString("PostgresDefaultConnection")
                ));

        serviceCollection.AddLogging();

        serviceCollection.AddScoped<IRepository<AudioFile>, AudioFileRepository>();
        serviceCollection.AddScoped<IRepository<Artist>, ArtistRepository>();
        serviceCollection.AddScoped<IRepository<Album>, AlbumRepository>();
        serviceCollection.AddScoped<IRepository<Track>, TrackRepository>();
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddScoped<IEmailSender, EmailSender>();

        ServiceProvider = serviceCollection.BuildServiceProvider();

        Seed();
    }

    private void Seed() {
        lock (_lock) {
            if (_databaseInitialized) {
                return;
            }

            using var context = ServiceProvider.GetService<AudioBoosContext>();
            if (context is null) {
                throw new NullException("Unable to get db context");
            }

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SaveChanges();
            _databaseInitialized = true;
        }
    }
}

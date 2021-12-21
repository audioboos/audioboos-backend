using System;
using System.Collections.Generic;
using System.Linq;
using AudioBoos.Data.Extensions;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AudioBoos.Data;

public class AudioBoosContext : IdentityDbContext<AppUser> {
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<AudioFile> AudioFiles { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<TrackPlayLog> TrackPlayLogs { get; set; }


    public AudioBoosContext(DbContextOptions options) : base(options) {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder
            .UseSnakeCaseNamingConvention()
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.HasDefaultSchema("app");
        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes()) {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)) {
                builder.Entity(entityType.ClrType)
                    .Property<DateTime>(nameof(BaseEntity.CreateDate))
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                builder.Entity(entityType.ClrType)
                    .Property<DateTime>(nameof(BaseEntity.UpdateDate))
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            }

            if (typeof(BaseAudioEntity).IsAssignableFrom(entityType.ClrType)) {
                builder.Entity(entityType.ClrType)
                    .Property<List<string>>(nameof(BaseAudioEntity.AlternativeNames))
                    .HasConversion(v => string.Join("|", v),
                        v => v.Split(new[] {'|'}).ToList(),
                        new ValueComparer<List<string>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()
                        ));
            }
        }

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in builder.Model.GetEntityTypes()) {
            if (entityType.IsKeyless) {
                continue;
            }

            foreach (var property in entityType.GetProperties()) {
                if (property.ClrType == typeof(DateTime)) {
                    property.SetValueConverter(dateTimeConverter);
                } else if (property.ClrType == typeof(DateTime?)) {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }

        builder.Entity<AppUser>(userEntity => {
            userEntity.ToTable("users");
        });

        builder.Entity<Artist>()
            .Property(a => a.AlternativeNames)
            .HasConversion(v => string.Join("|", v),
                v => v.Split(new[] {'|'}).ToList(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));

        builder.Entity<Artist>()
            .Property(e => e.Aliases)
            .HasConversion(v => string.Join("|", v),
                v => v.Split(new[] {'|'}).ToList(),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));

        foreach (var entity in builder.Model.GetEntityTypes()
                     .Where(p => p.Name.StartsWith("Microsoft.AspNetCore.Identity") ||
                                 p.Name.StartsWith("IdentityServer4.EntityFramework.Entities"))
                     .Select(p => builder.Entity(p.ClrType))) {
            if (entity.Metadata.ClrType.Name.Contains("AspNet")) {
                Console.WriteLine("Here");
            }

            entity.ToTable(entity.Metadata.ClrType.Name.Replace("`1", ""), "auth");
        }

        builder.CreateUniqueKeys();
        builder.SeedRoles();
    }

    private IEnumerable<PropertyBuilder> __getColumn(ModelBuilder modelBuilder, string columnName) {
        return modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.Name == columnName)
            .Select(p => modelBuilder.Entity(p.DeclaringEntityType.ClrType).Property(p.Name));
    }
}

using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Store;
using Flurl;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AudioBoos.Server.Services.Startup;

public static class MappingStartup {
    public static IServiceCollection AddMapping(this IServiceCollection services, IConfiguration config) {
        var smallWidth = config["ImageOptions:SmallImageWidth"];
        var smallHeight = config["ImageOptions:SmallImageHeight"];
        var largeWidth = config["ImageOptions:LargeImageWidth"];
        var largeHeight = config["ImageOptions:LargeImageHeight"];

        TypeAdapterConfig<AppUser, ProfileDto>
            .NewConfig()
            .Map(dest => dest.Email, src => src.Email);

        TypeAdapterConfig<ProfileDto, AppUser>
            .NewConfig();

        TypeAdapterConfig<Artist, ArtistDto>
            .NewConfig()
        .Map(dest => dest.Id, src => src.Id.ToString())
        .Map(dest => dest.Name, src => src.Name)
        .Map(dest => dest.NormalisedName, src => src.GetNormalisedName())
        .Map(dest => dest.FirstSeen, src => src.CreateDate)
        .Map(dest => dest.SmallImage,
            src =>
                $"{config.GetSection("System").GetValue<string>("BaseUrl")}/images/artist/{src.Id}.jpg?width={smallWidth}&height={smallHeight}")
        .Map(dest => dest.LargeImage,
            src =>
                $"{config.GetSection("System").GetValue<string>("BaseUrl")}/images/artist/{src.Id}.jpg?width={largeWidth}&height={largeHeight}");


        TypeAdapterConfig<AlbumDto, Album>
            .NewConfig()
            .Ignore(r => r.SmallImage)
            .Ignore(r => r.LargeImage);

        TypeAdapterConfig<Album, AlbumDto>
            .NewConfig()
            .MapToConstructor(false)
            .ConstructUsing(src => new AlbumDto(src.Artist.Name))
            .Map(dest => dest.Id, src => src.Id.ToString())
            .Map(dest => dest.ArtistName, src => src.Artist.Name)
            .Map(dest => dest.SmallImage,
                src =>
                    $"{config.GetSection("System").GetValue<string>("BaseUrl")}/images/album/{src.Id}.jpg?width={smallWidth}&height={smallHeight}")
            .Map(dest => dest.LargeImage,
                src =>
                    $"{config.GetSection("System").GetValue<string>("BaseUrl")}/images/album/{src.Id}.jpg?width={largeWidth}&height={largeHeight}")
            .Map(dest => dest.ReleaseDate,
                src => src.ReleaseDate ?? src.UpdateDate)
            .PreserveReference(true);
        ;

        TypeAdapterConfig<TrackDto, Track>
            .NewConfig()
            .ConstructUsing(src => new Track(src.AlbumName));

        TypeAdapterConfig<Track, TrackDto>
            .NewConfig()
            .ConstructUsing(src => new TrackDto(src.Album.Name))
            .Map(dest => dest.Id, src => src.Id.ToString())
            .Map(dest => dest.AudioUrl,
                src => new Url(config.GetSection("System").GetValue<string>("StreamUrl"))
                    .SetQueryParams(new {
                        trackId = src.Id
                    },
                        NullValueHandling.Remove));

        TypeAdapterConfig<TrackPlayLog, TrackPlayDto>
            .NewConfig()
            .Map(dest => dest.DatePlayed, src => src.UpdateDate)
            .Map(dest => dest.PlayedByIp, src => src.IPAddress.ToString())
            .Map(dest => dest.PlayedByUser, src => src.User.UserName);

        return services;
    }
}

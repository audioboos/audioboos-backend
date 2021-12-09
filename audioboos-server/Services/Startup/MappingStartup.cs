using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Store;
using Flurl;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AudioBoos.Server.Services.Startup;

public static class MappingStartup {
    public static IServiceCollection AddMapping(this IServiceCollection services, IConfiguration config) {
        TypeAdapterConfig<ArtistDto, Artist>
            .NewConfig()
            .ConstructUsing(src => new Artist(src.Name));

        TypeAdapterConfig<Artist, ArtistDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id.ToString())
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.NormalisedName, src => src.GetNormalisedName())
            .Map(dest => dest.SmallImage,
                src =>
                    $"{config.GetSection("System").GetValue<string>("BaseUrl")}/images/artist/{src.Id}.jpg?width=32&height=32")
            .Map(dest => dest.LargeImage,
                src =>
                    $"{config.GetSection("System").GetValue<string>("BaseUrl")}/images/artist/{src.Id}.jpg?width=320&height=220");


        TypeAdapterConfig<AlbumDto, Album>
            .NewConfig()
            .ConstructUsing(src => new Album(src.Name));

        TypeAdapterConfig<Album, AlbumDto>
            .NewConfig()
            .ConstructUsing(src => new AlbumDto(src.Artist.Name))
            .Map(dest => dest.Id, src => src.Id.ToString())
            .Map(dest => dest.SmallImage,
                src =>
                    $"{config.GetSection("System").GetValue<string>("BaseUrl")}/images/album/{src.Id}.jpg?width=32&height=32")
            .Map(dest => dest.LargeImage,
                src =>
                    $"{config.GetSection("System").GetValue<string>("BaseUrl")}/images/album/{src.Id}.jpg?width=320&height=220");

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

        return services;
    }
}

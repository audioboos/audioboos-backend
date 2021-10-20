using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Store;
using Flurl;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AudioBoos.Server.Services.Startup {
    public static class MappingStartup {
        public static IServiceCollection AddMapping(this IServiceCollection services, IConfiguration config) {
            TypeAdapterConfig<ArtistDTO, Artist>
                .NewConfig()
                .ConstructUsing(src => new Artist(src.Name));

            TypeAdapterConfig<Artist, ArtistDTO>
                .NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.SmallImage,
                    src => string.IsNullOrEmpty(src.SmallImage)
                        ? "https://generative-placeholders.glitch.me/image?width=64&height=64&style=mondrian"
                        : src.SmallImage)
                .Map(dest => dest.LargeImage,
                    src => string.IsNullOrEmpty(src.LargeImage)
                        ? "https://generative-placeholders.glitch.me/image?width=600&height=300&style=mondrian"
                        : src.SmallImage);

            TypeAdapterConfig<AlbumDTO, Album>
                .NewConfig()
                .ConstructUsing(src => new Album(src.Name));

            TypeAdapterConfig<Album, AlbumDTO>
                .NewConfig()
                .ConstructUsing(src => new AlbumDTO(src.Artist.Name))
                .Map(dest => dest.SmallImage, 
                    src => $"{config.GetSection("System").GetValue<string>("BaseUrl")}/image/album/{src.Id}?type=small")
                .Map(dest => dest.SmallImage, 
                    src => $"{config.GetSection("System").GetValue<string>("BaseUrl")}/image/album/{src.Id}?type=large");

            TypeAdapterConfig<TrackDTO, Track>
                .NewConfig()
                .ConstructUsing(src => new Track(src.AlbumName));

            TypeAdapterConfig<Track, TrackDTO>
                .NewConfig()
                .ConstructUsing(src => new TrackDTO(src.Album.Name))
                .Map(dest => dest.AudioUrl,
                    src => new Url(config.GetSection("System").GetValue<string>("StreamUrl"))
                        .SetQueryParams(new {
                                trackId = src.Id
                            },
                            NullValueHandling.Remove));

            return services;
        }
    }
}

using System;
using Flurl;
using Hqub.MusicBrainz.API.Services;

namespace AudioBoos.Server.Helpers;

public static class MusicBrainzExtensions {
    public static string GetQueryUrl(this IReleaseService release, string entity, string query, int limit = 25,
        int offset = 0) => Flurl.Url.Combine(
        "https://musicbrainz.org/ws/2/",
        $"{entity}?query={query}&limit={limit}&offset={offset}&fmt=json"
    );
}

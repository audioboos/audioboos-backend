using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AudioBoos.Server.Services.Utils;

public static class ResourceReader {
    public static async Task<Stream> ReadResourceAsByteArray(string resourceName) {
        var assembly = Assembly.GetExecutingAssembly();

        var resourceStream = assembly.GetManifestResourceStream($"AudioBoos.Server.Resources.{resourceName}");
        return await Task.FromResult(resourceStream);
    }
}

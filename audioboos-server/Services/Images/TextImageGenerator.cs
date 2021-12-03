using System.IO;
using System.Threading.Tasks;
using CliWrap;

namespace AudioBoos.Server.Services.Images;

public static class TextImageGenerator {
    public static async Task<byte[]> CreateArtistAvatarImage(string artistName) {
        var outputFile = $"{Path.GetTempFileName()}.png";

        var args = $@"-background gray -fill azure \
            -font Roboto-Regular -gravity Center \
            -size 64x64 caption:{artistName.TrimStart('T', 'h', 'e').Trim().Substring(0, 1)} \
            {outputFile}";

        var result = await Cli.Wrap("/usr/bin/convert")
            .WithArguments(args)
            .WithWorkingDirectory(Path.GetTempPath())
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        var fileContent = await File.ReadAllBytesAsync(outputFile);
        File.Delete(outputFile);
        return fileContent;
    }

    public static async Task<byte[]> CreateArtistImage(string artistName) {
        var outputFile = $"{Path.GetTempFileName()}.png";
        var args = $@"-background gray -fill azure \
            -font Roboto-Regular -gravity Center \
            -size 300x200 caption:{artistName} \
            {outputFile}";
        var result = await Cli.Wrap("/usr/bin/convert")
            .WithArguments(args)
            .WithWorkingDirectory(Path.GetTempPath())
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        var fileContent = await File.ReadAllBytesAsync(outputFile);
        File.Delete(outputFile);
        return fileContent;
    }

    public static async Task<byte[]> CreateAlbumImage(string artistName, string albumName) {
        var text = $"{artistName}\n{albumName}";
        var outputFile = $"{Path.GetTempFileName()}.png";
        var args = $@"-background gray -fill azure \
            -font Roboto-Regular -gravity Center \
            -size 300x200 caption:'{text} \
            {outputFile}";
        var result = await Cli.Wrap("/usr/bin/convert")
            .WithArguments(args)
            .WithWorkingDirectory(Path.GetTempPath())
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        var fileContent = await File.ReadAllBytesAsync(outputFile);
        File.Delete(outputFile);
        return fileContent;
    }
}

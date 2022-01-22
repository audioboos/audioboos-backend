using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace AudioBoos.Server.Helpers;

public static class StreamExtensions {
    public static async Task<byte[]> ToBytes(this Stream stream, CancellationToken cancellationToken = default) {
        byte[] bytes = new byte[stream.Length];
        stream.Position = 0;
        await stream.ReadAsync(bytes.AsMemory(0, (int)stream.Length), cancellationToken);
        return bytes;
    }

    public static String ToEncodedString(this Stream stream, Encoding enc = null) {
        enc = enc ?? Encoding.UTF8;

        byte[] bytes = new byte[stream.Length];
        stream.Position = 0;
        stream.Read(bytes, 0, (int)stream.Length);
        string data = enc.GetString(bytes);

        return enc.GetString(bytes);
    }

    public static async Task<string> SaveToLocalFile(this Stream stream, string fileName,
        CancellationToken cancellationToken = default) {
        var bytes = await stream.ToBytes(cancellationToken);
        await File.WriteAllBytesAsync(fileName, bytes, cancellationToken);
        return fileName;
    }

    public static async Task<string> ToBase64StringAsync(this Stream stream,
        CancellationToken cancellationToken = default) {
        var bytes = await stream.ToBytes(cancellationToken);
        return Convert.ToBase64String(bytes);
    }
}

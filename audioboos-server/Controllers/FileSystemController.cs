using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FileSystemController : ControllerBase {
    private readonly List<string> _exclusions = new() {
        "/dev",
        "/lib",
        "/lib64",
        "/etc",
        "/bin",
        "/sbin",
        "/bin",
        "/proc",
        "/usr",
        "/root",
        "/lost+found",
        "/boot",
        "/run",
        "/var",
        "/sys"
    };

    [HttpGet("directories")]
    public async Task<List<string>> GetMachineFolders([FromQuery] string path = "/") {
        return await Task.Run(() => {
            var folders = Directory
                .EnumerateDirectories(path, "*")
                .Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden & FileAttributes.System) == 0);

            return folders
                .Except(_exclusions)
                .Select(r => r)
                .OrderBy(r => r)
                .ToList();
        });
    }
}

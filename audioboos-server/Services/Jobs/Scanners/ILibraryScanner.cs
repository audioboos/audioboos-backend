using System.Threading;
using System.Threading.Tasks;

namespace AudioBoos.Server.Services.Jobs.Scanners {
    public interface ILibraryScanner {
        Task<(int, int, int)> ScanLibrary(CancellationToken cancellationToken);
    }
}

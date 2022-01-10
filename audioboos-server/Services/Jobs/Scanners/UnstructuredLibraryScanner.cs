using System;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data;
using AudioBoos.Data.Interfaces;
using AudioBoos.Data.Store;
using AudioBoos.Server.Services.AudioLookup;
using AudioBoos.Server.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace AudioBoos.Server.Services.Jobs.Scanners;

/// <summary>
/// This provides a better scanner for "unstructured" audio libraries
/// Best guess would be to index all audio files and work backwards from there 
/// </summary>
internal class UnstructuredLibraryScanner : LibraryScanner {
    public UnstructuredLibraryScanner(ILogger<UnstructuredLibraryScanner> logger,
        IHubContext<JobHub> messageClient,
        IAudioLookupService lookupService,
        ISchedulerFactory schedulerFactory,
        IServiceProvider serviceProvider) : base(logger, messageClient, lookupService, schedulerFactory,
        serviceProvider) {
    }

    public override async Task<(int, int, int)> ScanLibrary(bool deepScan, string childFolder,
        CancellationToken cancellationToken) {
        _logger.LogInformation("Starting unstructured library scan");
        return await Task.FromResult((0, 0, 0));
    }
}

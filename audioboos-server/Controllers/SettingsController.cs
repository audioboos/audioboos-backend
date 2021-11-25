using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SettingsController : ControllerBase {
    private readonly AudioBoosContext _context;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly SystemSettings _systemSettings;

    public SettingsController(IOptions<SystemSettings> systemSettings, AudioBoosContext context,
        ISchedulerFactory schedulerFactory) {
        _context = context;
        _schedulerFactory = schedulerFactory;
        _systemSettings = systemSettings.Value;
    }

    [HttpGet]
    public async Task<ActionResult<SettingsDto>> Get() {
        var settings = await _context
            .Settings
            .FirstOrDefaultAsync(r => r.Key.ToLower().Equals("sitename"));

        return new SettingsDto(
            settings is not null ? settings.Value : string.Empty);
    }

    [HttpPost]
    public async Task<ActionResult<SettingsDto>> CreateInitialSettings(InitialSettingsDto initialSettings,
        CancellationToken cancellationToken) {
        foreach (PropertyInfo prop in initialSettings.GetType().GetProperties()) {
            var value = prop.GetValue(initialSettings)?.ToString();
            if (string.IsNullOrEmpty(value))
                continue;

            var existing = await _context
                .Settings
                .FirstOrDefaultAsync(e => e.Key.Equals(prop.Name),
                    cancellationToken);
            if (existing is not null) {
                existing.Value = value;
            } else {
                await _context.Settings.AddAsync(
                    new Setting(prop.Name, value),
                    cancellationToken);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(new JobKey("UpdateLibrary", "DEFAULT"), cancellationToken);

        return new SettingsDto(
            (await _context
                .Settings
                .FindAsync(new object[] {"SiteName"}, cancellationToken))?.Value ?? _systemSettings.DefaultSiteName
        );
    }
}

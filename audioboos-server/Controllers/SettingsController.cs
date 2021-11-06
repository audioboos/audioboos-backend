using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Persistence;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SettingsController : ControllerBase {
    private readonly AudioBoosContext _context;
    private readonly SystemSettings _systemSettings;

    public SettingsController(IOptions<SystemSettings> systemSettings, AudioBoosContext context) {
        _context = context;
        _systemSettings = systemSettings.Value;
    }

    [HttpGet]
    public ActionResult<SettingsDto> Get() {
        return new SettingsDto(_systemSettings.DefaultSiteName);
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
        return new SettingsDto(
            (await _context
                .Settings
                .FindAsync(new object[] {"SiteName"}, cancellationToken))?.Value ?? _systemSettings.DefaultSiteName
        );
    }
}

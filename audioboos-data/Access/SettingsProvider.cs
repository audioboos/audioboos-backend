using AudioBoos.Data.Persistence;
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Access;

/// <summary>
/// And I should probably use a custom configuration provider for this
/// but they suck and this works so - bite me!
/// </summary>
public class SettingsProvider {
    private readonly DbSet<Setting> _settings;

    protected SettingsProvider(AudioBoosContext context) {
        this._settings = context.Settings;
    }

    public async Task<string?> Get(string key) => await _settings
        .Where(s => s.Key.Equals(key))
        .Select(r => r.Value)
        .FirstOrDefaultAsync();
}

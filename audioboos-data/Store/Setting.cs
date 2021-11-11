using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Store;

[Index(nameof(Key), IsUnique = true)]
public class Setting {
    public Setting(string key, string value) {
        Key = key;
        Value = value;
    }

    [Key] public string Key { get; set; }
    public string Value { get; set; }
}

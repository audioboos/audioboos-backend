using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Store;

[Index(nameof(key), IsUnique = true)]
public record Setting(string key, string value);

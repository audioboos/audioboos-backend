using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace AudioBoos.Data.Store;

public class AppUser : IdentityUser {
    [JsonIgnore] public List<RefreshToken> RefreshTokens { get; set; } = new();
}

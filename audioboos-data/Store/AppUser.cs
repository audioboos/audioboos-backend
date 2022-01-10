using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AudioBoos.Data.Store;

public class AppUser : IdentityUser {
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? Description { get; set; }
    public string? Photo { get; set; }


    [JsonIgnore] public List<RefreshToken> RefreshTokens { get; set; } = new();
}

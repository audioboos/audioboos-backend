using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AudioBoos.Data.Annotations;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Store;

public record RefreshToken : BaseEntity {
    [UniqueKey] public string Token { get; set; }
    [UniqueKey] public string JwtToken { get; set; }
    public bool Revoked { get; set; } = false;

    public AppUser User { get; set; }
}

using AudioBoos.Data.Annotations;

namespace AudioBoos.Data.Store;

public record RefreshToken : BaseEntity {
    [UniqueKey] public string Token { get; set; }
    [UniqueKey] public string JwtToken { get; set; }
    public bool Revoked { get; set; } = false;

    public AppUser User { get; set; }
}

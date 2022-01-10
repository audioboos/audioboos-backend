#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AudioBoos.Data.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AudioBoos.Data.Store;

public abstract record BaseEntity {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [AdaptIgnore]
    public Guid Id { get; set; }

    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}

public abstract record BaseAudioEntity() : BaseEntity, IBaseAudioEntity {
    protected BaseAudioEntity(string name) : this() {
        Name = name;
    }

    [Required] public string Name { get; set; }
    public string? Description { get; set; }

    [Required] public TaggingStatus TaggingStatus { get; set; } = TaggingStatus.None;

    public List<string> AlternativeNames { get; set; } = new();

    public DateTime FirstScanDate { get; set; }
    public DateTime LastScanDate { get; set; }

    // Rider thinks this can be protected as only the derived classes access it
    // Once Rider understands static abstract interface methods we can remove this
    // ReSharper disable once MemberCanBeProtected.Global 
    public static bool IsIncomplete(IBaseAudioEntity audioEntity) =>
        audioEntity is not null &&
        !string.IsNullOrEmpty((audioEntity as BaseAudioEntity)?.Name) &&
        !string.IsNullOrEmpty((audioEntity as BaseAudioEntity)?.Description);
}

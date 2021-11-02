#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AudioBoos.Data.Persistence.Interfaces;
using Mapster;

namespace AudioBoos.Data.Store;

public abstract record BaseEntity(
    [Required] string Name) : IBaseEntity {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [AdaptIgnore]
    public Guid Id { get; set; }

    public string? Description { get; set; }

    [Required] public TaggingStatus TaggingStatus { get; set; } = TaggingStatus.None;

    public List<string> AlternativeNames { get; set; } = new();

    public DateTime FirstScanDate { get; set; }
    public DateTime LastScanDate { get; set; }

    // Rider thinks this can be protected as only the derived classes access it
    // Once Rider understands static abstract interface methods we can remove this
    // ReSharper disable once MemberCanBeProtected.Global 
    public static bool IsIncomplete(IBaseEntity entity) =>
        entity is not null &&
        !string.IsNullOrEmpty((entity as BaseEntity)?.Name) &&
        !string.IsNullOrEmpty((entity as BaseEntity)?.Description);
}

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AudioBoos.Data.Persistence.Interfaces;

namespace AudioBoos.Data.Store {
    public abstract record BaseEntity(
        [Required] string Name) : IBaseEntity {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string? Description { get; set; }

        [Required] public TaggingStatus TaggingStatus { get; set; } = TaggingStatus.None;

        public List<string> AlternativeNames { get; set; } = new();

        public DateTime FirstScanDate { get; set; }
        public DateTime LastScanDate { get; set; }
    }
}

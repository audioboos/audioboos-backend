using System;
using System.ComponentModel.DataAnnotations;
using AudioBoos.Data.Annotations;

namespace AudioBoos.Data.Store; 

public record Track : BaseAudioEntity {
    public Track(string name) : base(name) {
    }

    public Track(Guid albumId, string name, int trackNumber, string path) : base(name) {
        this.AlbumId = albumId;
        this.TrackNumber = trackNumber;
        this.PhysicalPath = path;
    }

    [Required] public int TrackNumber { get; set; }
    public string? Comments { get; set; }
    public string? AudioUrl { get; set; }

    [Required] [UniqueKey] public string PhysicalPath { get; set; }

    [Required] public Guid AlbumId { get; set; }
    [Required] public Album Album { get; set; }

    [Required] public DateTime ScanDate { get; set; }
}
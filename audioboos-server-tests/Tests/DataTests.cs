using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Persistence;
using AudioBoos.Data.Store;
using AudioBoos.Server.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AudioBoos.Server.Tests;

public class DataTests : IClassFixture<DbFixture> {
    private readonly ServiceProvider _serviceProvider;

    public DataTests(DbFixture fixture) {
        _serviceProvider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task Test_Insert_AudioFile_Raw() {
        using var scope = _serviceProvider.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AudioBoosContext>();

        var audioFile = new AudioFile(
            "/tmp/Test_Insert_AudioFile_Raw_audio.mp3",
            "Test_Insert_AudioFile_Raw_Artist",
            "Test_Insert_AudioFile_Raw_Album",
            "Test_Insert_AudioFile_Raw_Track");
        Assert.True(audioFile.Id.Equals(Guid.Empty));
        context.AudioFiles.Add(audioFile);
        await context.SaveChangesAsync();

        Assert.True(!audioFile.Id.Equals(Guid.Empty));
    }

    [Fact]
    public async Task Test_Insert_AudioFile_Repository() {
        using var scope = _serviceProvider.CreateScope();
        var audioFileRepository = scope.ServiceProvider.GetRequiredService<IRepository<AudioFile>>();

        var audioFile = await audioFileRepository.InsertOrUpdate(
            new AudioFile(
                "/tmp/Test_Insert_AudioFile_Repository_audio.mp3",
                "Test_Insert_AudioFile_Repository_Artist",
                "Test_Insert_AudioFile_Repository_Album",
                "Test_Insert_AudioFile_Repository_Track"));
        audioFile.Checksum = "Pre Save Test Checksum";
        await audioFileRepository.Context.SaveChangesAsync();
        Assert.Equal(1, audioFileRepository.Context.AudioFiles.Count());
        Assert.Equal(1, audioFileRepository.GetAll().Count());
        audioFile.Checksum = "Post Save Test Checksum";

        await audioFileRepository.InsertOrUpdate(audioFile);

        var test = await audioFileRepository.GetByFile("/tmp/Test_Insert_AudioFile_Repository_audio.mp3");
        Assert.Equal("Post Save Test Checksum", test?.Checksum);
    }

    [Fact]
    public async Task Test_Insert_Artist() {
        using var scope = _serviceProvider.CreateScope();
        var artistRepository = scope.ServiceProvider.GetRequiredService<IRepository<Artist>>();

        var artist = await artistRepository.InsertOrUpdate(
            new Artist("Test_Insert_Artist_Artist"));
        artist.Description = "Pre Save Test Description";
        await artistRepository.Context.SaveChangesAsync();
        Assert.Equal(1, artistRepository.Context.Artists.Count());
        Assert.Equal(1, artistRepository.GetAll().Count());
        artist.Description = "Post Save Test Description";

        await artistRepository.InsertOrUpdate(artist);
        await artistRepository.Context.SaveChangesAsync();

        var test = await artistRepository.GetByName("Test_Insert_Artist_Artist");
        Assert.Equal("Post Save Test Description", test?.Description);
    }

    [Fact]
    public async Task Test_AlternativeNames() {
        using var scope = _serviceProvider.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AudioBoosContext>();

        var artist = new Artist("Test_AlternativeNames_Artist") {
            Description = "Test_AlternativeNames_Artist_Description",
            AlternativeNames = new List<string> {"Alt 1", "Alt 2"}
        };

        context.Artists.Add(artist);
        await context.SaveChangesAsync();
        var saved = await context.Artists.Where(a => a.Id.Equals(artist.Id)).FirstOrDefaultAsync();
        Assert.True(saved?.AlternativeNames.Count == 2);
    }
}

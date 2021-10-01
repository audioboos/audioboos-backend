using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Persistence;
using AudioBoos.Data.Persistence.Interfaces;
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AudioBoos.Server.Tests {
    public class DataTests : IClassFixture<DbFixture> {
        private ServiceProvider _serviceProvider;

        public DataTests(DbFixture fixture) {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async Task Test_Insert_AudioFile() {
            await using var context = _serviceProvider.GetService<AudioBoosContext>();
            if (context is null) {
                Assert.False(true, "Unable to get context");
                return;
            }

            var audioFile = new AudioFile(
                "/tmp/audio.mp3",
                "Artist",
                "Album",
                "Track");
            Assert.True(audioFile.Id.Equals(Guid.Empty));
            context.AudioFiles.Add(audioFile);
            await context.SaveChangesAsync();

            Assert.True(!audioFile.Id.Equals(Guid.Empty));
        }

        [Fact]
        public async Task Test_Upsert_AudioFile() {
            var audioFileRepository = _serviceProvider.GetService<IRepository<AudioFile>>();
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
            if (audioFileRepository is null || unitOfWork is null) {
                Assert.False(true, "Unable to get AudioFileRepository");
                return;
            }

            var audioFile = new AudioFile(
                "/tmp/audio.mp3",
                "Artist",
                "Album",
                "Track");
            audioFile.Checksum = "INITIAL";
            Assert.True(audioFile.Id.Equals(Guid.Empty));

            var newRecord = await audioFileRepository.InsertOrUpdate(audioFile);
            await unitOfWork.Complete();

            Assert.False(audioFile.Id.Equals(Guid.Empty));

            newRecord.Checksum = "POST";
            var upserted = await audioFileRepository.InsertOrUpdate(audioFile);
            Assert.True(upserted.Checksum.Equals("POST"));

            var found = await audioFileRepository.GetByFile(audioFile.PhysicalPath);
            Assert.True(found?.Checksum.Equals("POST"));
        }

        [Fact]
        public async Task Test_Insert() {
            var audioFileRepository = _serviceProvider.GetService<IRepository<AudioFile>>();
            var artistRepository = _serviceProvider.GetService<IRepository<Artist>>();
            var trackRepository = _serviceProvider.GetService<IRepository<Track>>();
            var albumRepository = _serviceProvider.GetService<IRepository<Album>>();
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
            await using var context = _serviceProvider.GetService<AudioBoosContext>();

            if (audioFileRepository is null || artistRepository is null || trackRepository is null ||
                albumRepository is null || unitOfWork is null || context is null) {
                Assert.False(true, "Unable to get repositories from DI");
                return;
            }

            var audioFile = await audioFileRepository.InsertOrUpdate(
                new AudioFile(
                    "/tmp/audio.mp3",
                    "Artist",
                    "Album",
                    "Track"));
            await unitOfWork.Complete();

            var artist = await artistRepository.InsertOrUpdate(new Artist("Test Artist One") {
                Description = "Test Artist One Description",
            });

            var album = await albumRepository.InsertOrUpdate(new Album(artist, "Test Artist One - Album One"));

            await unitOfWork.Complete();

            Assert.True(context.AudioFiles.Where(
                    i => i.Id.Equals(audioFile.Id))?.Count() == 1,
                "One Audio File Inserted");

            Assert.True(context.Artists.Count().Equals(1), "One Artist Inserted");
            Assert.True(context.Albums.Count().Equals(1), "One Album Inserted");
            Assert.True(context.Tracks.Count().Equals(1), "One Track Inserted");
        }

        [Fact]
        public async Task Test_AlternativeNames() {
            await using var context = _serviceProvider.GetService<AudioBoosContext>();
            if (context is null) {
                Assert.False(true, "Unable to get context");
                return;
            }

            var artist = new Artist("Test Artist One") {
                Description = "Test Artist One Description",
                AlternativeNames = new List<string> {"Alt 1", "Alt 2"}
            };

            context.Artists.Add(artist);
            await context.SaveChangesAsync();
            var saved = await context.Artists.Where(a => a.Id.Equals(artist.Id)).FirstOrDefaultAsync();
            Assert.True(saved?.AlternativeNames.Count == 2);
        }
    }
}

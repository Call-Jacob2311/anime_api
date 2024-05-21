using Moq;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using anime_api_shared.Models.Anime;
using anime_api_shared.Repositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using Moq.Dapper;

namespace anime_api_tests.Repositories
{
    [TestFixture]
    public class AnimeRepositoryTests
    {
        private Mock<IConfiguration> _configurationMock;
        private Mock<SqlConnection> _sqlConnectionMock;
        private AnimeRepository _animeRepository;

        [SetUp]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(config => config["ConnectionStrings:DefaultConnection"])
                .Returns("FakeConnectionString");

            _sqlConnectionMock = new Mock<SqlConnection>("FakeConnectionString");

            _animeRepository = new AnimeRepository(_configurationMock.Object);

            // Initialize the Serilog logger for testing
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .CreateLogger();
        }

        [Test]
        public async Task GetAnimeAsync_AnimeExists_ReturnsAnime()
        {
            // Arrange
            var animeName = "Naruto";
            var expectedAnime = new AnimeGetModel { AnimeName = animeName };

            _sqlConnectionMock.Setup(c => c.QueryFirstOrDefaultAsync<AnimeGetModel>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType>()))
                .ReturnsAsync(expectedAnime);

            // Act
            var result = await _animeRepository.GetAnimeAsync(animeName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedAnime.AnimeName, result.AnimeName);
        }

        [Test]
        public void GetAnimeAsync_ThrowsSqlException_LogsError()
        {
            SqlException sqlException = null;
            // Arrange
            var animeName = "Naruto";

            _sqlConnectionMock.Setup(c => c.QueryFirstOrDefaultAsync<AnimeGetModel>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType>()))
                .ThrowsAsync(sqlException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<SqlException>(() => _animeRepository.GetAnimeAsync(animeName));
            Assert.IsInstanceOf<SqlException>(ex);
        }

        [Test]
        public async Task AddAnimeAsync_ValidModel_ReturnsSuccess()
        {
            // Arrange
            var model = new AnimePostModel
            {
                AnimeName = "Naruto",
                AnimeStatus = "Ongoing",
                StudioId = 1,
                ReleaseDate = DateTime.Now,
                EpisodeCount = 220,
                Genres = "Action"
            };

            _sqlConnectionMock.Setup(c => c.QueryAsync<int>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType>()))
                .ReturnsAsync(new[] { 1 });

            // Act
            var result = await _animeRepository.AddAnimeAsync(model);

            // Assert
            Assert.AreEqual("Success!", result);
        }

        [Test]
        public void AddAnimeAsync_ThrowsSqlException_LogsError()
        {
            SqlException sqlException = null;
            // Arrange
            var model = new AnimePostModel
            {
                AnimeName = "Naruto",
                AnimeStatus = "Ongoing",
                StudioId = 1,
                ReleaseDate = DateTime.Now,
                EpisodeCount = 220,
                Genres = "Action"
            };

            _sqlConnectionMock.Setup(c => c.QueryAsync<int>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType>()))
                .ThrowsAsync(sqlException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<SqlException>(() => _animeRepository.AddAnimeAsync(model));
            Assert.IsInstanceOf<SqlException>(ex);
        }
    }
}

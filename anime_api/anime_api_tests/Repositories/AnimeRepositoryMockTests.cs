using anime_api.Services;
using anime_api_shared.Models.Anime;
using anime_api_shared.Repositories;
using Dapper;
using Moq;
using Moq.Dapper;
using System.Data;

namespace anime_api_tests.Repositories
{
    [TestFixture]
    public class AnimeRepositoryTests
    {
        private Mock<IDbConnectionFactory> _dbConnectionFactoryMock;
        private Mock<IDbConnection> _dbConnectionMock;
        private AnimeRepository _animeRepository;

        [SetUp]
        public void Setup()
        {
            _dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
            _dbConnectionMock = new Mock<IDbConnection>();
            _dbConnectionFactoryMock.Setup(factory => factory.CreateConnectionAsync())
                .ReturnsAsync(_dbConnectionMock.Object);

            _animeRepository = new AnimeRepository(_dbConnectionFactoryMock.Object);
        }

        [Test]
        public async Task GetAnimeAsync_ShouldReturnAnime_WhenAnimeExists()
        {
            // Arrange
            string animeName = "Naruto";
            var expectedAnime = new AnimeGetModel { AnimeName = animeName };
            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<AnimeGetModel>(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(expectedAnime);

            // Act
            var result = await _animeRepository.GetAnimeAsync(animeName);

            // Assert
            Assert.That(expectedAnime.AnimeName, Is.EqualTo(result.AnimeName));
        }

        [Test]
        public async Task GetAllAnimeAsync_ShouldReturnAnimeList()
        {
            // Arrange
            List<AnimeGetModel> animeGetModels = [new AnimeGetModel {AnimeName = "Naruto"}, new AnimeGetModel {AnimeName="One Piece"}];
            _dbConnectionMock.SetupDapperAsync(c => c.QueryAsync<AnimeGetModel>(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(animeGetModels);

            // Act
            var result = await _animeRepository.GetAllAnimeAsync();

            // Assert
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public async Task AddAnimeAsync_ShouldReturnSuccess_WhenAnimeAreAdded()
        {
            // Arrange
            var anime1 = new AnimePostModel
            {
              AnimeName = "sword art online",
              AnimeStatusId = 1,
              StudioId = 1,
              ReleaseDate = DateTime.Now.AddDays(-10),
              EpisodeCount = 12,
              Genres = "action, romance",
              ThumbnailLink = "string",
              SeasonId = 1,
              AnimeOSTId = 1,
              TrueName = "Sword Art Online",
              WikiUrl = "string",
              ShortHandNames = "SAO"
            };
            _dbConnectionMock.SetupDapperAsync(c => c.ExecuteAsync("AddAnime", It.IsAny<DynamicParameters>(), null, null, CommandType.StoredProcedure))
                .ReturnsAsync(1);

            // Act
            var result = await _animeRepository.AddAnimeAsync(anime1);

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.FirstOrDefault().Key, Does.Contain("Success"));
        }

        [Test]
        public async Task AddAnimeBulkAsync_ShouldReturnSuccess_WhenMultipleAnimeAreAdded()
        {
            // Arrange
            var anime1 = new AnimePostModel
            {
                AnimeName = "One Piece",
                AnimeStatusId = 1,
                StudioId = 1,
                ReleaseDate = DateTime.Now,
                EpisodeCount = 1000,
                Genres = "Action, Adventure"
            };
            var anime2 = new AnimePostModel
            {
                AnimeName = "Naruto",
                AnimeStatusId = 1,
                StudioId = 1,
                ReleaseDate = DateTime.Now,
                EpisodeCount = 1000,
                Genres = "Action, Adventure"
            };
            List<AnimePostModel> animePostModels = [anime1, anime2];
            _dbConnectionMock.SetupDapperAsync(c => c.ExecuteAsync("AddAnime", It.IsAny<DynamicParameters>(), null, null, CommandType.StoredProcedure))
                .ReturnsAsync(1);

            // Act
            var result = await _animeRepository.AddAnimeBulkAsync(animePostModels);

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.FirstOrDefault().Key, Does.Contain("Success"));
        }

        [Test]
        public async Task UpdateAnimeAsync_ShouldReturnSuccess_WhenAnimeAreUpdated()
        {
            // Arrange
            var anime1 = new AnimePutModel
            {
                AnimeId = 1,
                AnimeName = "One Piece",
                AnimeStatus = "Ongoing",
                StudioId = 1,
                ReleaseDate = DateTime.Now,
                EpisodeCount = 1000,
                Genres = "Action, Adventure"
            };
            _dbConnectionMock.SetupDapperAsync(c => c.ExecuteAsync("UpdateAnime", It.IsAny<DynamicParameters>(), null, null, CommandType.StoredProcedure))
                .ReturnsAsync(1);

            // Act
            var result = await _animeRepository.UpdateAnimeAsync(anime1);

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.FirstOrDefault().Key, Does.Contain("Success"));
        }

        [Test]
        public async Task UpdateAnimeBulkAsync_ShouldReturnSuccess_WhenMultipleAnimeAreUpdated()
        {
            // Arrange
            var anime1 = new AnimePutModel
            {
                AnimeId = 1,
                AnimeName = "One Piece",
                AnimeStatus = "Ongoing",
                StudioId = 1,
                ReleaseDate = DateTime.Now,
                EpisodeCount = 1000,
                Genres = "Action, Adventure"
            };
            var anime2 = new AnimePutModel
            {
                AnimeId = 2,
                AnimeName = "Naruto",
                AnimeStatus = "Completed",
                StudioId = 1,
                ReleaseDate = DateTime.Now,
                EpisodeCount = 1000,
                Genres = "Action, Adventure"
            };
            List<AnimePutModel> animePutModels = [anime1, anime2];
            _dbConnectionMock.SetupDapperAsync(c => c.ExecuteAsync("AddAnime", It.IsAny<DynamicParameters>(), null, null, CommandType.StoredProcedure))
                .ReturnsAsync(1);

            // Act
            var result = await _animeRepository.UpdateAnimeBulkAsync(animePutModels);

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.FirstOrDefault().Key, Does.Contain("Success"));
        }

        [Test]
        public async Task DeleteAnimeAsync_ShouldReturnSuccess_WhenAnimeIsDeleted()
        {
            // Arrange
            var animeName = "Naruto";
            _dbConnectionMock.SetupDapperAsync(c => c.ExecuteAsync("DeleteAnime", It.IsAny<DynamicParameters>(), null, null, CommandType.StoredProcedure))
                .ReturnsAsync(1);

            // Act
            var result = await _animeRepository.DeleteAnimeAsync(animeName);

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.FirstOrDefault().Key, Does.Contain("Success"));
        }

        [Test]
        public async Task DeleteAnimeBulkAsync_ShouldReturnSuccess_WhenAnimeListIsDeleted()
        {
            // Arrange
            List<string> animeList = new List<string> { "Naruto", "One Piece" };
            _dbConnectionMock.SetupDapperAsync(c => c.ExecuteAsync("DeleteAnime", It.IsAny<DynamicParameters>(), null, null, CommandType.StoredProcedure))
                .ReturnsAsync(1);

            // Act
            var result = await _animeRepository.DeleteAnimeBulkAsync(animeList);

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.FirstOrDefault().Key, Does.Contain("Success"));
        }

        //[Test]
        //public void AddAnimeAsync_ShouldLogErrorAndThrow_WhenSqlExceptionOccurs()
        //{
        //    // Arrange
        //    var anime = new AnimePostModel
        //    {
        //        AnimeName = "One Piece",
        //        AnimeStatus = "Ongoing",
        //        StudioId = 1,
        //        ReleaseDate = DateTime.Now,
        //        EpisodeCount = 1000,
        //        Genres = "Action, Adventure"
        //    };
        //    var exception = new Exception("Generic error");
        //    _dbConnectionMock.SetupDapperAsync(c => c.ExecuteAsync("AddAnime", It.IsAny<DynamicParameters>(), null, null, CommandType.StoredProcedure))
        //        .ThrowsAsync(exception);

        //    // Act & Assert
        //    var ex = Assert.ThrowsAsync<Exception>(async () => await _animeRepository.AddAnimeAsync(anime));
        //    Assert.That(exception.Message, Is.EqualTo(ex.Message));
        //}
    }
}

using Moq;
using Serilog;
using System.Data;
using anime_api_shared.Models.Anime;
using anime_api_shared.Repositories;
using Dapper;
using Moq.Dapper;
using anime_api.Services;
using static Dapper.SqlMapper;

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
            _dbConnectionFactoryMock.Setup(x => x.CreateConnectionAsync()).ReturnsAsync(_dbConnectionMock.Object);
            _animeRepository = new AnimeRepository(_dbConnectionFactoryMock.Object);

            // Initialize the Serilog logger for testing
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.Console()
                .CreateLogger();
        }

        #region Anime Tests
        [Test]
        public async Task GetAnimeAsync_AnimeExists_ReturnsAnime()
        {
            // Arrange
            var animeName = "Naruto";
            var expectedAnime = new AnimeGetModel { AnimeName = animeName };

            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<AnimeGetModel>(
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
            Assert.That(result.AnimeName, Is.EqualTo(expectedAnime.AnimeName));
        }

        [Test]
        public async Task GetAnimeAsync_AnimeDoesntExists_ReturnsNotFound()
        {
            // Arrange
            var animeName = "Naruto";
            var expectedAnime = new AnimeGetModel();

            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<AnimeGetModel>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType>()))
                .ReturnsAsync(expectedAnime);

            // Act
            var result = await _animeRepository.GetAnimeAsync(animeName);

            // Assert
            Assert.That(string.IsNullOrEmpty(result.AnimeName));
        }

        // Having issues mocking the SQL error exceptions. Might look into it in the future, for now, moving on

        //[Test]
        //public void GetAnimeAsync_ThrowsSqlException_LogsError()
        //{
        //    // Arrange
        //    var animeName = "Naruto";
        //    var sqlException = (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        //    _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<AnimeGetModel>(
        //            It.IsAny<string>(),
        //            It.IsAny<object>(),
        //            It.IsAny<IDbTransaction>(),
        //            It.IsAny<int?>(),
        //            It.IsAny<CommandType>()))
        //        .ThrowsAsync(sqlException);

        //    // Act & Assert
        //    var ex = Assert.ThrowsAsync<SqlException>(() => _animeRepository.GetAnimeAsync(animeName));
        //    Assert.IsInstanceOf<SqlException>(ex);
        //}

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

            var parameters = new DynamicParameters();
            parameters.Add("@response", 1, DbType.Int32, ParameterDirection.ReturnValue);
            parameters.Add("@animeName", model.AnimeName, DbType.String);
            parameters.Add("@animeStatus", model.AnimeStatus, DbType.String);
            parameters.Add("@studioId", model.StudioId, DbType.Int16);
            parameters.Add("@releaseDate", model.ReleaseDate, DbType.Date);
            parameters.Add("@episodeCount", model.EpisodeCount, DbType.Int16);
            parameters.Add("@genres", model.Genres, DbType.String);
            parameters.Add("@recordUpdated", DateTime.Now, DbType.DateTime);
            parameters.Add("@updatedBy", "Jacob C.", DbType.String);

            // Mock ExecuteAsync to simulate setting the output parameter
            _dbConnectionMock.SetupDapperAsync(c => c.ExecuteAsync("UpdateAnime", parameters, null, null, CommandType.StoredProcedure))
                             .ReturnsAsync(1);

            parameters.AddDynamicParams(new { response = 1 });

            // Act
            var result = await _animeRepository.AddAnimeAsync(model);

            // Assert
            Assert.That(result, Is.EqualTo("Success"));
        }

        [Test]
        public async Task UpdateAnimeAsync_ShouldReturnSuccess_WhenResponseIs1()
        {
            // Arrange
            var model = new AnimePutModel
            {
                AnimeName = "Naruto",
                AnimeStatus = "Ongoing",
                StudioId = 1,
                ReleaseDate = DateTime.Now,
                EpisodeCount = 500,
                Genres = "Action"
            };

            // Mock ExecuteAsync to simulate setting the output parameter
            _dbConnectionMock.SetupDapperAsync(c => c.ExecuteAsync("UpdateAnime", It.IsAny<DynamicParameters>(), null, null, CommandType.StoredProcedure))
                             .ReturnsAsync(1);

            // Act
            var result = await _animeRepository.UpdateAnimeAsync(model);

            // Assert
            Assert.That(result, Is.EqualTo("Success"));
        }

        // Having issues mocking the SQL error exceptions. Might look into it in the future, for now, moving on

        //[Test]
        //public void AddAnimeAsync_ThrowsSqlException_LogsError()
        //{
        //    // Arrange
        //    var model = new AnimePostModel
        //    {
        //        AnimeName = "Naruto",
        //        AnimeStatus = "Ongoing",
        //        StudioId = 1,
        //        ReleaseDate = DateTime.Now,
        //        EpisodeCount = 220,
        //        Genres = "Action"
        //    };

        //    // Create a mock of SqlException
        //    var sqlExceptionMock = new Mock<SqlException>("Mock SQL error message", MockBehavior.Default);

        //    // Setup the properties or behavior of the mocked SqlException
        //    sqlExceptionMock.SetupGet(e => e.Message).Returns("Mock SQL error message");
        //    sqlExceptionMock.SetupGet(e => e.Number).Returns(12345); // Mock SQL error number
        //    // You can continue setting up other properties or behaviors as needed

        //    // Use the mocked SqlException in your test
        //    var exception = sqlExceptionMock.Object;

        //    _dbConnectionMock.SetupDapperAsync(c => c.QueryAsync<int>(
        //            It.IsAny<string>(),
        //            It.IsAny<object>(),
        //            It.IsAny<IDbTransaction>(),
        //            It.IsAny<int?>(),
        //            It.IsAny<CommandType>()))
        //        .ThrowsAsync(exception);

        //    // Act & Assert
        //    var ex = Assert.ThrowsAsync<SqlException>(() => _animeRepository.AddAnimeAsync(model));
        //    Assert.IsInstanceOf<SqlException>(ex);
        //}
    }
    #endregion
}

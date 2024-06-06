using anime_api.Services;
using anime_api_shared.Models.Anime;
using Dapper;
using Serilog;
using System.Data;
using System.Data.SqlClient;

namespace anime_api_shared.Repositories
{
    /// <summary>
    /// Interface for Anime repository.
    /// </summary>
    public interface IAnimeRepository
    {
        /// <summary>
        /// Retrieves the details of an anime by its name.
        /// </summary>
        /// <param name="animeName">The name of the anime.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the AnimeGetModel.</returns>
        Task<AnimeGetModel> GetAnimeAsync(string animeName);

        /// <summary>
        /// Retrieves the details of all anime.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of AnimeGetModel.</returns>
        Task<List<AnimeGetModel>> GetAllAnimeAsync();

        /// <summary>
        /// Adds a new anime to the repository.
        /// </summary>
        /// <param name="anime">The anime details to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary with the result of the operation.</returns>
        Task<Dictionary<string, string>> AddAnimeAsync(AnimePostModel anime);

        /// <summary>
        /// Adds multiple new anime to the repository.
        /// </summary>
        /// <param name="animeList">The list of anime details to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary with the result of the operation.</returns>
        Task<Dictionary<string, string>> AddAnimeBulkAsync(List<AnimePostModel> animeList);

        /// <summary>
        /// Updates the details of an existing anime.
        /// </summary>
        /// <param name="model">The anime details to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary with the result of the operation.</returns>
        Task<Dictionary<string, string>> UpdateAnimeAsync(AnimePutModel model);

        /// <summary>
        /// Updates the details of multiple existing anime.
        /// </summary>
        /// <param name="animeList">The list of anime details to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary with the result of the operation.</returns>
        Task<Dictionary<string, string>> UpdateAnimeBulkAsync(List<AnimePutModel> animeList);

        /// <summary>
        /// Deletes an anime by its name.
        /// </summary>
        /// <param name="animeName">The name of the anime to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the delete operation.</returns>
        Task<Dictionary<string, string>> DeleteAnimeAsync(string animeName);

        /// <summary>
        /// Deletes a string list of anime by name.
        /// </summary>
        /// <param name="animeList">The anime list to be deleted.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the delete operation.</returns>
        Task<Dictionary<string, string>> DeleteAnimeBulkAsync(List<string> animeList);
    }

    /// <summary>
    /// Implementation of the IAnimeRepository interface.
    /// </summary>
    public class AnimeRepository : IAnimeRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeRepository"/> class.
        /// </summary>
        /// <param name="dbConnectionFactory">The database connection factory.</param>
        public AnimeRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
        }

        #region GET
        /// <inheritdoc />
        public async Task<AnimeGetModel> GetAnimeAsync(string animeName)
        {
            using (var connection = await _dbConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@animeName", animeName, DbType.String);

                    var result = await connection.QueryFirstOrDefaultAsync<AnimeGetModel>("GetAnimeByName", parameters, commandType: CommandType.StoredProcedure);

                    return result ?? new AnimeGetModel();
                }
                catch (SqlException ex)
                {
                    Log.Error(ex, "Error fetching anime details for: {AnimeName}", animeName);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unexpected error fetching anime details for: {AnimeName}", animeName);
                    throw;
                }
            }
        }

        /// <inheritdoc />
        public async Task<List<AnimeGetModel>> GetAllAnimeAsync()
        {
            using (var connection = await _dbConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    List<AnimeGetModel> animeGetModels = [];
                    var parameters = new DynamicParameters();

                    var result = await connection.QueryAsync<AnimeGetModel>("GetAllAnime", parameters, commandType: CommandType.StoredProcedure);
                    foreach (var item in result)
                    {
                        animeGetModels.Add(item);
                    }

                    return animeGetModels;
                }
                catch (SqlException ex)
                {
                    Log.Error(ex, "Error fetching all anime details.");
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error fetching all anime details.");
                    throw;
                }
            }
        }
        #endregion

        #region POST
        /// <inheritdoc />
        public async Task<Dictionary<string, string>> AddAnimeAsync(AnimePostModel anime)
        {
            var results = new Dictionary<string, string>();
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            try
            {
                DynamicParameters parameters = new();
                parameters.Add("@animeName", anime.AnimeName, DbType.String);
                parameters.Add("@animeStatus", anime.AnimeStatus, DbType.String);
                parameters.Add("@studioId", anime.StudioId, DbType.Int16);
                parameters.Add("@releaseDate", anime.ReleaseDate, DbType.Date);
                parameters.Add("@episodeCount", anime.EpisodeCount, DbType.Int16);
                parameters.Add("@genres", anime.Genres, DbType.String);
                parameters.Add("@recordCreation", DateTime.Now, DbType.DateTime);
                parameters.Add("@createdBy", "Jacob C.", DbType.String);
                parameters.Add("@response", DbType.Int32, direction: ParameterDirection.ReturnValue); // Output of the stored procedure gets saved into this param.

                var execute = await connection.ExecuteAsync("AddAnime", parameters, commandType: CommandType.StoredProcedure);
                results.Add("Success: " + anime.AnimeName, "Successfully updated the record: " + anime.AnimeName);

                return results;
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "Error creating anime details for: {AnimeName}", anime.AnimeName);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error creating anime details for: {AnimeName}", anime.AnimeName);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>> AddAnimeBulkAsync(List<AnimePostModel> animeList)
        {
            var results = new Dictionary<string, string>();
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            try
            {
                foreach (var anime in animeList)
                {
                    DynamicParameters parameters = new();
                    parameters.Add("@animeName", anime.AnimeName, DbType.String);
                    parameters.Add("@animeStatus", anime.AnimeStatus, DbType.String);
                    parameters.Add("@studioId", anime.StudioId, DbType.Int16);
                    parameters.Add("@releaseDate", anime.ReleaseDate, DbType.Date);
                    parameters.Add("@episodeCount", anime.EpisodeCount, DbType.Int16);
                    parameters.Add("@genres", anime.Genres, DbType.String);
                    parameters.Add("@recordCreation", DateTime.Now, DbType.DateTime);
                    parameters.Add("@createdBy", "Jacob C.", DbType.String);
                    parameters.Add("@response", DbType.Int32, direction: ParameterDirection.ReturnValue); // Output of the stored procedure gets saved into this param.

                    var execute = await connection.ExecuteAsync("AddAnime", parameters, commandType: CommandType.StoredProcedure);
                    results.Add("Success: " + anime.AnimeName, "Successfully created the record: " + anime.AnimeName);
                }
                return results;
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "Error creating multiple anime details. Please validate and try again.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating multiple anime details. Please validate and try again.");
                throw;
            }
        }
        #endregion

        #region PUT
        /// <inheritdoc />
        public async Task<Dictionary<string, string>> UpdateAnimeAsync(AnimePutModel anime)
        {
            var results = new Dictionary<string, string>();
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@animeId", anime.AnimeId, DbType.Int32);
                parameters.Add("@animeName", anime.AnimeName, DbType.String);
                parameters.Add("@animeStatus", anime.AnimeStatus, DbType.String);
                parameters.Add("@studioId", anime.StudioId, DbType.Int16);
                parameters.Add("@releaseDate", anime.ReleaseDate, DbType.Date);
                parameters.Add("@episodeCount", anime.EpisodeCount, DbType.Int16);
                parameters.Add("@genres", anime.Genres, DbType.String);
                parameters.Add("@recordUpdated", DateTime.Now, DbType.DateTime);
                parameters.Add("@updatedBy", "Jacob C.", DbType.String);
                parameters.Add("@response", 1, DbType.Int32, direction: ParameterDirection.ReturnValue); // Output of the stored procedure gets saved into this param.

                var execute = await connection.ExecuteAsync("UpdateAnime", parameters, commandType: CommandType.StoredProcedure);
                results.Add("Success: " + anime.AnimeName, "Successfully updated the record: " + anime.AnimeName);

                return results;
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "Error updating anime details for: {AnimeName}", anime.AnimeName);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error updating anime details for: {AnimeName}", anime.AnimeName);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>> UpdateAnimeBulkAsync(List<AnimePutModel> animeList)
        {
            var results = new Dictionary<string, string>();
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            try
            {
                foreach (var anime in animeList)
                {
                    DynamicParameters parameters = new();
                    parameters.Add("@animeId", anime.AnimeId, DbType.Int32);
                    parameters.Add("@animeName", anime.AnimeName, DbType.String);
                    parameters.Add("@animeStatus", anime.AnimeStatus, DbType.String);
                    parameters.Add("@studioId", anime.StudioId, DbType.Int16);
                    parameters.Add("@releaseDate", anime.ReleaseDate, DbType.Date);
                    parameters.Add("@episodeCount", anime.EpisodeCount, DbType.Int16);
                    parameters.Add("@genres", anime.Genres, DbType.String);
                    parameters.Add("@recordCreation", DateTime.Now, DbType.DateTime);
                    parameters.Add("@createdBy", "Jacob C.", DbType.String);
                    parameters.Add("@response", DbType.Int32, direction: ParameterDirection.ReturnValue); // Output of the stored procedure gets saved into this param.

                    var execute = await connection.ExecuteAsync("UpdateAnime", parameters, commandType: CommandType.StoredProcedure);
                    results.Add("Success: " + anime.AnimeName, "Successfully updated the record: " + anime.AnimeName);
                }
                return results;
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "Error updating multiple anime details. Please validate and try again.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating multiple anime details. Please validate and try again.");
                throw;
            }
        }
        #endregion

        #region DELETE
        /// <inheritdoc />
        public async Task<Dictionary<string, string>> DeleteAnimeAsync(string animeName)
        {
            var results = new Dictionary<string, string>();
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@animeName", animeName, DbType.String);

                var execute = await connection.ExecuteAsync("DeleteAnimeByName", parameters, commandType: CommandType.StoredProcedure);
                results.Add("Success: " + animeName, "Successfully deleted the record: " + animeName);
                return results;
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "Error deleting anime details for: {AnimeName}", animeName);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error deleting anime details for: {AnimeName}", animeName);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>> DeleteAnimeBulkAsync(List<string> animeList)
        {
            var results = new Dictionary<string, string>();
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            try
            {
                foreach (var animeName in animeList)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@animeName", animeName, DbType.String);

                    var execute = await connection.ExecuteAsync("DeleteAnimeByName", parameters, commandType: CommandType.StoredProcedure);
                    results.Add("Success: " + animeName, "Successfully deleted the record: " + animeName);
                }
                
                return results;
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "Error deleting anime list.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error deleting anime list.");
                throw;
            }
        }
        #endregion
    }
}
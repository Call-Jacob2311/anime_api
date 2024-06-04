using anime_api.Services;
using anime_api_shared.Models.Anime;
using Dapper;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace anime_api_shared.Repositories
{
    public interface IAnimeRepository
    {
        Task<AnimeGetModel> GetAnimeAsync(string animeName);
        Task<List<AnimeGetModel>> GetAllAnimeAsync();
        Task<Dictionary<string, string>> AddAnimeAsync(AnimePostModel anime);
        Task<Dictionary<string, string>> AddAnimeBulkAsync(List<AnimePostModel> animeList);
        Task<string> UpdateAnimeAsync(AnimePutModel model);
        Task<string> DeleteAnimeAsync(string animeName);
    }

    public class AnimeRepository : IAnimeRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public AnimeRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
        }

        #region GET
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

                results.Add("Success: " + anime.AnimeName, "Successfully created the record: " + anime.AnimeName);

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
                    var isSuccess = parameters.Get<int>("@response");

                    if (isSuccess == 1)
                    {
                        results.Add("Success: " + anime.AnimeName, "Successfully created the record: " + anime.AnimeName);
                    }
                    else
                    {
                        results.Add("Failure: " + anime.AnimeName, "Failed to create record for: " + anime.AnimeName);
                    }
                    
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
        public async Task<string> UpdateAnimeAsync(AnimePutModel model)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@animeId", model.AnimeId, DbType.Int32);
                parameters.Add("@animeName", model.AnimeName, DbType.String);
                parameters.Add("@animeStatus", model.AnimeStatus, DbType.String);
                parameters.Add("@studioId", model.StudioId, DbType.Int16);
                parameters.Add("@releaseDate", model.ReleaseDate, DbType.Date);
                parameters.Add("@episodeCount", model.EpisodeCount, DbType.Int16);
                parameters.Add("@genres", model.Genres, DbType.String);
                parameters.Add("@recordUpdated", DateTime.Now, DbType.DateTime);
                parameters.Add("@updatedBy", "Jacob C.", DbType.String);
                // Output of the stored procedure gets saved into this param.
                parameters.Add("@response", 1, DbType.Int32, direction: ParameterDirection.ReturnValue);

                var execute = await connection.ExecuteAsync("UpdateAnime", parameters, commandType: CommandType.StoredProcedure);

                return "Success";
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "Error updating anime details for: {AnimeName}", model.AnimeName);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error updating anime details for: {AnimeName}", model.AnimeName);
                throw;
            }
        }
        #endregion

        #region DELETE
        public async Task<string> DeleteAnimeAsync(string animeName)
        {
            using (var connection = await _dbConnectionFactory.CreateConnectionAsync())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@animeName", animeName, DbType.String);

                    var result = await connection.ExecuteAsync("DeleteAnimeByName", parameters, commandType: CommandType.StoredProcedure);

                    return "Success";
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
        }
        #endregion
    }
}
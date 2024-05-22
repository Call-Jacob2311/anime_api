using anime_api.Services;
using anime_api_shared.Models.Anime;
using Dapper;
using Serilog;
using System.Data;
using System.Data.SqlClient;

namespace anime_api_shared.Repositories
{
    public interface IAnimeRepository
    {
        Task<AnimeGetModel> GetAnimeAsync(string animeName);
        Task<string> AddAnimeAsync(AnimePostModel model);
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

        public async Task<string> AddAnimeAsync(AnimePostModel model)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@animeName", model.AnimeName, DbType.String);
                parameters.Add("@animeStatus", model.AnimeStatus, DbType.String);
                parameters.Add("@studioId", model.StudioId, DbType.Int16);
                parameters.Add("@releaseDate", model.ReleaseDate, DbType.Date);
                parameters.Add("@episodeCount", model.EpisodeCount, DbType.Int16);
                parameters.Add("@genres", model.Genres, DbType.String);
                parameters.Add("@recordCreation", DateTime.Now, DbType.DateTime);
                parameters.Add("@createdBy", "Jacob C.", DbType.String);
                // Output of the stored procedure gets saved into this param.
                parameters.Add("@response", DbType.Int32, direction: ParameterDirection.ReturnValue);

                var execute = await connection.ExecuteAsync("AddAnime", parameters, commandType: CommandType.StoredProcedure);

                return "Success";
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "Error creating anime details for: {AnimeName}", model.AnimeName);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error creating anime details for: {AnimeName}", model.AnimeName);
                throw;
            }
        }

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
    }
}
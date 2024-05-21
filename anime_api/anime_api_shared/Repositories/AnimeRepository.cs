using anime_api_shared.Models.Anime;
using Dapper;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace anime_api_shared.Repositories
{
    public interface IAnimeRepository
    {
        Task<AnimeGetModel> GetAnimeAsync(string animeName);
        Task<string> AddAnimeAsync(AnimePostModel model);
    }

    public class AnimeRepository : IAnimeRepository
    {
        private readonly string? _connectionString;

        public AnimeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
        }

        public async Task<AnimeGetModel> GetAnimeAsync(string animeName)
        {
            await using var connection = new SqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync();

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

        public async Task<string> AddAnimeAsync(AnimePostModel model)
        {
            await using var connection = new SqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@animeName", model.AnimeName, DbType.String);
                parameters.Add("@animeStatus", model.AnimeStatus, DbType.String);
                parameters.Add("@studioId", model.StudioId, DbType.Int16);
                parameters.Add("@releaseDate", model.ReleaseDate, DbType.Date);
                parameters.Add("@episodeCount", model.EpisodeCount, DbType.Int16);
                parameters.Add("@genres", model.Genres, DbType.String);
                parameters.Add("@recordCreation", DateTime.Now, DbType.DateTime);
                parameters.Add("@createdBy", "Jacob C.", DbType.String); // Consider making this dynamic

                var result = await connection.QueryAsync<int>("AddAnime", parameters, commandType: CommandType.StoredProcedure);

                return "Success!";
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
    }
}
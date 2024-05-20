using anime_api_shared.Models.Anime;

namespace anime_api_shared.Repositories
{
    public interface IAnimeRepository
    {
        Task<AnimeGetModel> GetAnimeAsync(string animeName);
        Task<string> AddAnimeAsync(string animeName);
    }

    public class AnimeRepository : IAnimeRepository {
        // Add any dependency injections here 
        public AnimeRepository() { 

        }

        public async Task<AnimeGetModel> GetAnimeAsync(string animeName)
        {
            return new AnimeGetModel();
        }

        public async Task<string> AddAnimeAsync(string animeName)
        {
            return await AddAnimeAsync(animeName);
        }
    }
}

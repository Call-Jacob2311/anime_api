using anime_api_shared.Models.Anime;
using anime_api_shared.Repositories;

namespace anime_api_shared.Services
{
    public interface IAnimeService
    {
        Task<AnimeGetModel> GetAnimeAsync(string animeName);
        Task<List<AnimeGetModel>> GetAllAnimeAsync();
        Task<Dictionary<string, string>> AddAnimeAsync(AnimePostModel anime);
        Task<Dictionary<string, string>> AddAnimeBulkAsync(List<AnimePostModel> animeList);
        Task<Dictionary<string, string>> UpdateAnimeAsync(AnimePutModel model);
        Task<Dictionary<string, string>> UpdateAnimeBulkAsync(List<AnimePutModel> animeList);
        Task<string> DeleteAnimeAsync(string animeName);
    }

    public class AnimeService : IAnimeService
    {
        private readonly IAnimeRepository _repository;

        public AnimeService(IAnimeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<AnimeGetModel> GetAnimeAsync(string animeName)
        {
            if (string.IsNullOrEmpty(animeName))
            {
                throw new ArgumentException("Anime name cannot be null or empty.", nameof(animeName));
            }

            return await _repository.GetAnimeAsync(animeName);
        }

        public async Task<List<AnimeGetModel>> GetAllAnimeAsync()
        {
            return await _repository.GetAllAnimeAsync();
        }

        public async Task<Dictionary<string, string>> AddAnimeAsync(AnimePostModel anime)
        {
            if (anime == null)
            {
                throw new ArgumentException("Payload model cannot be null or empty.", nameof(anime));
            }

            return await _repository.AddAnimeAsync(anime);
        }

        public async Task<Dictionary<string, string>> AddAnimeBulkAsync(List<AnimePostModel> animeList)
        {
            if (animeList == null)
            {
                throw new ArgumentException("Payload model cannot be null or empty.", nameof(animeList));
            }

            return await _repository.AddAnimeBulkAsync(animeList);
        }

        public async Task<Dictionary<string, string>> UpdateAnimeAsync(AnimePutModel anime)
        {
            if (anime == null)
            {
                throw new ArgumentException("Payload model cannot be null or empty.", nameof(anime));
            }

            return await _repository.UpdateAnimeAsync(anime);
        }

        public async Task<Dictionary<string, string>> UpdateAnimeBulkAsync(List<AnimePutModel> animeList)
        {
            if (animeList == null)
            {
                throw new ArgumentException("Payload model cannot be null or empty.", nameof(animeList));
            }

            return await _repository.UpdateAnimeBulkAsync(animeList);
        }

        public async Task<string> DeleteAnimeAsync(string animeName)
        {
            if (string.IsNullOrEmpty(animeName))
            {
                throw new ArgumentException("Anime name cannot be null or empty.", nameof(animeName));
            }

            return await _repository.DeleteAnimeAsync(animeName);
        }
    }
}

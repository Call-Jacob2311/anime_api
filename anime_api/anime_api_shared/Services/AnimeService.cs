﻿using anime_api_shared.Models.Anime;
using anime_api_shared.Repositories;

namespace anime_api_shared.Services
{
    public interface IAnimeService
    {
        Task<AnimeGetModel> GetAnimeAsync(string animeName);
        Task<string> AddAnimeAsync(string animeName);
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

        public async Task<string> AddAnimeAsync(string animeName)
        {
            if (string.IsNullOrEmpty(animeName))
            {
                throw new ArgumentException("Anime name cannot be null or empty.", nameof(animeName));
            }

            return await _repository.AddAnimeAsync(animeName);
        }
    }
}
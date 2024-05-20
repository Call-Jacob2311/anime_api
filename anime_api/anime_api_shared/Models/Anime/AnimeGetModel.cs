namespace anime_api_shared.Models.Anime
{
    public class AnimeGetModel
    {
        /// <summary>
        /// Gets or sets the name of the anime.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status of the anime (e.g., Ongoing, Completed).
        /// </summary>
        public string AnimeStatus { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the studio that produced the anime.
        /// </summary>
        public string StudioName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the release date of the anime.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the number of episodes in the anime.
        /// </summary>
        public int EpisodeCount { get; set; }

        /// <summary>
        /// Gets or sets the genres of the anime.
        /// </summary>
        public string Genres { get; set; } = string.Empty;
    }
}
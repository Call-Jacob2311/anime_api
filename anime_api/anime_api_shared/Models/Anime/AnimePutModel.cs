using System.ComponentModel.DataAnnotations;

namespace anime_api_shared.Models.Anime
{
    public class AnimePutModel
    {
        /// <summary>
        /// Gets or sets the id of the anime.
        /// </summary>
        public int AnimeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the anime.
        /// </summary>
        [Required]
        public string AnimeName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status of the anime (e.g., Ongoing, Completed).
        /// </summary>
        [Required]
        public string AnimeStatus { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the id of the studio that produced the anime.
        /// </summary>
        [Required]
        public int StudioId { get; set; }

        /// <summary>
        /// Gets or sets the release date of the anime.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the number of episodes in the anime.
        /// </summary>
        [Required]
        public int EpisodeCount { get; set; }

        /// <summary>
        /// Gets or sets the genres of the anime.
        /// </summary>
        [Required]
        public string Genres { get; set; } = string.Empty;
    }
}
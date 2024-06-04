using System;
using System.ComponentModel.DataAnnotations;

namespace anime_api_shared.Models.Anime
{
    public class AnimePostModel
    {
        /// <summary>
        /// Gets or sets the name of the anime.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Anime name can't be longer than 100 characters.")]
        public string AnimeName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status of the anime (e.g., Ongoing, Completed).
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Anime status can't be longer than 50 characters.")]
        public string AnimeStatus { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the id of the studio that produced the anime.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "StudioId must be greater than 0.")]
        public int StudioId { get; set; }

        /// <summary>
        /// Gets or sets the release date of the anime.
        /// </summary>
        public DateTime ReleaseDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the number of episodes in the anime.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Episode count must be greater than 0.")]
        public int EpisodeCount { get; set; }

        /// <summary>
        /// Gets or sets the genres of the anime.
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "Genres can't be longer than 200 characters.")]
        public string Genres { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;

namespace anime_api_shared.Models.Anime
{
    public class AnimeGetModel
    {
        /// <summary>
        /// Gets or sets the id of the anime.
        /// </summary>
        public int AnimeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the anime.
        /// </summary>
        public string AnimeName { get; set; } = string.Empty;

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

        /// <summary>
        /// Gets or sets the thumbnail link for the anime.
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "Thumb nail link can't be longer than 200 characters.")]
        public string ThumbnailLink { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the season for the anime.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Episode count must be greater than 0.")]
        public int SeasonId { get; set; }

        /// <summary>
        /// Gets or sets the ost for the anime.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Anime OST id count must be greater than 0.")]
        public int AnimeOSTId { get; set; }

        /// <summary>
        /// Gets or sets the true name (japanese) for the anime.
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "Thumb nail link can't be longer than 200 characters.")]
        public string TrueName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the wiki url for the anime.
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "Thumb nail link can't be longer than 200 characters.")]
        public string WikiUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the short hand names for the anime
        /// </summary>
        [Required]
        [StringLength(200, ErrorMessage = "Thumb nail link can't be longer than 200 characters.")]
        public string ShortHandNames { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeGetModel"/> class.
        /// </summary>
        public AnimeGetModel() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeGetModel"/> class with parameters.
        /// </summary>
        public AnimeGetModel(int animeId, string animeName, string animeStatus, string studioName, DateTime releaseDate, int episodeCount, string genres, string thumbNails, int seasonId, int animeOstId,
            string trueName, string wikiUrl, string shortHandNames)
        {
            AnimeId = animeId;
            AnimeName = animeName ?? throw new ArgumentNullException(nameof(animeName));
            AnimeStatus = animeStatus ?? throw new ArgumentNullException(nameof(animeStatus));
            StudioName = studioName ?? throw new ArgumentNullException(nameof(studioName));
            ReleaseDate = releaseDate;
            EpisodeCount = episodeCount;
            Genres = genres ?? throw new ArgumentNullException(nameof(genres));
            ThumbnailLink = thumbNails;
            SeasonId = seasonId;
            AnimeOSTId = animeOstId;
            TrueName = trueName;
            wikiUrl = wikiUrl ?? throw new ArgumentNullException(nameof(wikiUrl));
            ShortHandNames = shortHandNames;
        }
    }
}
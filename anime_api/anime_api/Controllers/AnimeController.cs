using anime_api.Models;
using anime_api_shared.Models.Anime;
using anime_api_shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace anime_api.Controllers
{
    /// <summary>
    /// API controller for handling anime data.
    /// </summary>
    [ApiController]
    [Route("api/v1/anime")]
    public class AnimeController : ControllerBase
    {
        private readonly IAnimeService _animeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeController"/> class.
        /// </summary>
        /// <param name="animeService">The anime service.</param>
        public AnimeController(IAnimeService animeService)
        {
            _animeService = animeService ?? throw new ArgumentNullException(nameof(animeService));
        }

        /// <summary>
        /// Gets the anime details from the database.
        /// </summary>
        /// <param name="animeName">The name of the anime.</param>
        /// <returns>The anime data model.</returns>
        [HttpGet("{animeName}")]
        [ProducesResponseType(typeof(AnimeGetModel), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> GetAnime(string animeName)
        {
            try
            {
                if (string.IsNullOrEmpty(animeName))
                {
                    return BadRequest("Anime name cannot be null or empty.");
                }

                // Converting to lower case to remove any future case senstive issues.
                var convertedName = animeName.ToLower();

                var result = await _animeService.GetAnimeAsync(convertedName);

                if (result == null)
                {
                    return NotFound($"{animeName} doesn't exist in the database. Please validate and try again.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                var exceptionErrorModel = new ExceptionErrorModel(ex.Message, ex.StackTrace);
                return BadRequest(exceptionErrorModel);
            }
        }
    }
}
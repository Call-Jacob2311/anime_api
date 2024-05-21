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
                // TODO: Add thumbnails for anime, start linking OSTs. Will need to update models and tests.

                //Validation.
                if (string.IsNullOrEmpty(animeName))
                {
                    return BadRequest("Anime name cannot be null or empty.");
                }

                // Converting to lower case to remove any future case senstive issues.
                var convertedName = animeName.ToLower();

                // Get result.
                var result = await _animeService.GetAnimeAsync(convertedName);

                // Flag for missing result
                if (string.IsNullOrEmpty(result.AnimeName))
                {
                    return NotFound($"{animeName} doesn't exist in the database. Please validate and try again.");
                }

                // Results
                return Ok(result);
            }
            catch (Exception ex)
            {
                var exceptionErrorModel = new ExceptionErrorModel(ex.Message, ex.StackTrace);
                return BadRequest(exceptionErrorModel);
            }
        }

        /// <summary>
        /// Add a anime to the database.
        /// </summary>
        /// <returns>Validation string for record creation.</returns>
        [HttpPost()]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> AddAnime(AnimePostModel model)
        {
            try
            {
                // TODO: Once I add endpoint to get Studios, need validation on the studio id. Also need to vlaidate zeros
                //Validation.
                if (model == null)
                {
                    return BadRequest("Model cannot be null or empty.");
                }

                // Make sure record doesn't already exist
                var checkName = await _animeService.GetAnimeAsync(model.AnimeName.ToLower());
                if (string.IsNullOrEmpty(checkName.AnimeName))
                {
                    // Add to db.
                    var result = await _animeService.AddAnimeAsync(model);

                    // Results
                    return Ok(result);
                } else
                {
                    return BadRequest(model.AnimeName + " already exists. Please validate and try again.");
                }  
            }
            catch (Exception ex)
            {
                var exceptionErrorModel = new ExceptionErrorModel(ex.Message, ex.StackTrace);
                return BadRequest(exceptionErrorModel);
            }
        }
    }
}
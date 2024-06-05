using anime_api.Models;
using anime_api.Models.Enums;
using anime_api_shared.Models.Anime;
using anime_api_shared.Models.ModelValidations;
using anime_api_shared.Services;
using FluentValidation;
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
        private readonly IValidator<AnimePostModel> _postValidator;
        private readonly IValidator<AnimePutModel> _putValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimeController"/> class.
        /// </summary>
        /// <param name="animeService">The anime service.</param>
        public AnimeController(IAnimeService animeService, IValidator<AnimePostModel> postValidator, IValidator<AnimePutModel> putValidator)
        {
            _animeService = animeService ?? throw new ArgumentNullException(nameof(animeService));
            _postValidator = postValidator ?? throw new ArgumentNullException(nameof(postValidator));
            _putValidator = putValidator ?? throw new ArgumentNullException(nameof(putValidator));
        }

        #region GET endpoints
        /// <summary>
        /// Retrieves the anime details from a database.
        /// </summary>
        /// <param name="animeName">The name of the anime.</param>
        /// <returns>The anime data model.</returns>
        [HttpGet("{animeName}")]
        [ProducesResponseType(typeof(AnimeGetModel), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> GetAnime([FromRoute] string animeName)
        {
            // Validaiton of param name
            if (string.IsNullOrEmpty(animeName))
            {
                return BadRequest("Anime name cannot be null or empty.");
            }

            // I like to save everything lower case to avoid future case sensitivity
            var convertedName = animeName.ToLower();

            var result = await _animeService.GetAnimeAsync(convertedName);

            if (string.IsNullOrEmpty(result?.AnimeName))
            {
                return NotFound($"{animeName} doesn't exist in the database. Please validate and try again.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all anime details from a database.
        /// </summary>
        /// <param name="startIndex">Starting point for retrieval.</param>
        /// <param name="pageSize">Number of records you want to retrieve.</param>
        /// <returns>The anime data model.</returns>
        [HttpGet("{startIndex:int}/{pageSize:int}")]
        [ProducesResponseType(typeof(IEnumerable<AnimeGetModel>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> GetAllAnime([FromRoute] int startIndex = 0, int pageSize = 50)
        {
            var result = await _animeService.GetAllAnimeAsync();

            // Validaiton of missing result
            if (result == null || !result.Any())
            {
                return NotFound("No data found.");
            }

            var paginatedResult = result.Skip(startIndex).Take(pageSize).ToList();

            return Ok(paginatedResult);
        }
        #endregion

        #region POST endpoints
        /// <summary>
        /// Add an anime to a database.
        /// </summary>
        /// <returns>Validation string for record creation.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(MultiResults), 200)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> AddAnime([FromBody] AnimePostModel anime)
        {
            // Validaiton of model
            var modelValidation = _postValidator.Validate(anime);
            if (!modelValidation.IsValid)
            {
                return BadRequest(modelValidation.Errors);
            }

            // Validaiton for existing record
            var checkName = await _animeService.GetAnimeAsync(anime.AnimeName.ToLower());
            if (string.IsNullOrEmpty(checkName?.AnimeName))
            {
                var result = await _animeService.AddAnimeAsync(anime);

                var finalResults = new MultiResults
                {
                    SuccessfulRecordsCount = result.Count(r => r.Key.Contains(ResponseStatus.Success.ToString())),
                    FailureRecordsCount = result.Count(r => r.Key.Contains(ResponseStatus.Failure.ToString())),
                    ResultList = result
                };

                return finalResults.SuccessfulRecordsCount > 0 ? Ok(finalResults) : BadRequest(finalResults);
            }
            else
            {
                return BadRequest($"{anime.AnimeName} already exists. Please validate and try again.");
            }
        }

        /// <summary>
        /// Add multiple anime to a database.
        /// </summary>
        /// <returns>Validation string for each record creation.</returns>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(MultiResults), 200)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> AddAnimeBulk([FromBody] List<AnimePostModel> animeList)
        {
            // Validate model
            var validator = new AnimePostModelValidator(animeList);
            foreach (var anime in animeList)
            {
                var validationResult = validator.Validate(anime);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
            }

            // Validate exisiting records
            foreach (var anime in animeList)
            {
                var checkName = await _animeService.GetAnimeAsync(anime.AnimeName.ToLower());
                if (!string.IsNullOrEmpty(checkName?.AnimeName))
                {
                    return BadRequest($"Duplicate record detected: {anime.AnimeName}. Please validate and try again.");
                }
            }

            var result = await _animeService.AddAnimeBulkAsync(animeList);

            var finalResults = new MultiResults
            {
                SuccessfulRecordsCount = result.Count(r => r.Key.Contains(ResponseStatus.Success.ToString())),
                FailureRecordsCount = result.Count(r => r.Key.Contains(ResponseStatus.Failure.ToString())),
                ResultList = result
            };

            return finalResults.SuccessfulRecordsCount > 0 ? Ok(finalResults) : BadRequest(finalResults);
        }
        #endregion

        #region PUT endpoints
        /// <summary>
        /// Update an anime in the database.
        /// </summary>
        /// <returns>Validation string for record update.</returns>
        [HttpPut]
        [ProducesResponseType(typeof(MultiResults), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> UpdateAnime([FromBody] AnimePutModel anime)
        {
            // Validaiton of model
            var modelValidation = _putValidator.Validate(anime);
            if (!modelValidation.IsValid)
            {
                return BadRequest(modelValidation.Errors);
            }

            var checkName = await _animeService.GetAnimeAsync(anime.AnimeName.ToLower());
            if (string.IsNullOrEmpty(checkName?.AnimeName) || anime.AnimeId != checkName.AnimeId)
            {
                var result = await _animeService.UpdateAnimeAsync(anime);

                var finalResults = new MultiResults
                {
                    SuccessfulRecordsCount = result.Count(r => r.Key.Contains(ResponseStatus.Success.ToString())),
                    FailureRecordsCount = result.Count(r => r.Key.Contains(ResponseStatus.Failure.ToString())),
                    ResultList = result
                };

                return finalResults.SuccessfulRecordsCount > 0 ? Ok(finalResults) : BadRequest(finalResults);
            }
            else
            {
                return NotFound($"{anime.AnimeName} doesn't exist. Please validate and try again.");
            }
        }

        /// <summary>
        /// Update multiple anime in the database.
        /// </summary>
        /// <returns>Validation string for each record update.</returns>
        [HttpPut("bulk")]
        [ProducesResponseType(typeof(MultiResults), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> UpdateAnimeBulk([FromBody] List<AnimePutModel> animeList)
        {
            // Validate model
            var validator = new AnimePutModelValidator(animeList);
            foreach (var anime in animeList)
            {
                var validationResult = validator.Validate(anime);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
            }

            // Validate exisiting records
            foreach (var anime in animeList)
            {
                var checkName = await _animeService.GetAnimeAsync(anime.AnimeName.ToLower());
                if (!string.IsNullOrEmpty(checkName?.AnimeName))
                {
                    return BadRequest($"Duplicate record detected: {anime.AnimeName}. Please validate and try again.");
                }
            }

            var result = await _animeService.UpdateAnimeBulkAsync(animeList);

            var finalResults = new MultiResults
            {
                SuccessfulRecordsCount = result.Count(r => r.Key.Contains(ResponseStatus.Success.ToString())),
                FailureRecordsCount = result.Count(r => r.Key.Contains(ResponseStatus.Failure.ToString())),
                ResultList = result
            };

            return finalResults.SuccessfulRecordsCount > 0 ? Ok(finalResults) : BadRequest(finalResults);
        }
        #endregion

        #region DELETE endpoints
        /// <summary>
        /// Soft delete an anime in the database.
        /// </summary>
        /// <returns>Validation string for record deletion.</returns>
        [HttpDelete("{animeName}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> DeleteAnime([FromRoute]string animeName)
        {
            if (string.IsNullOrEmpty(animeName))
            {
                return BadRequest("Anime name cannot be null or empty.");
            }

            var convertedName = animeName.ToLower();

            var getResult = await _animeService.GetAnimeAsync(convertedName);

            if (string.IsNullOrEmpty(getResult?.AnimeName))
            {
                return NotFound($"{animeName} doesn't exist in the database. Please validate and try again.");
            }

            var result = await _animeService.DeleteAnimeAsync(convertedName);

            return Ok(result);
        }
        #endregion
    }
}
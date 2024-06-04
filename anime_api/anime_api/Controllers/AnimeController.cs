using anime_api.Models;
using anime_api.Models.Enums;
using anime_api_shared.Models.Anime;
using anime_api_shared.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using System.Net;
using System.Reflection;
using System.Xml.Linq;

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
        /// Retrieves all anime details from a database.
        /// </summary>
        /// <param name="startIndex">Starting point for retrieval.</param>
        /// <param name="pageSize">Number of records you want to retrieve.</param>
        /// <returns>The anime data model.</returns>
        [HttpGet("{startIndex}&{pageSize}")]
        [ProducesResponseType(typeof(AnimeGetModel), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> GetAllAnime(int startIndex = 0, int pageSize = 50) // Default values
        {
            try
            {
                // Get result.
                var result = await _animeService.GetAllAnimeAsync(startIndex, pageSize);

                // Flag for missing result
                if (result == null)
                {
                    return BadRequest("Issue retrieving data. Please validate and try again.");
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
        #endregion

        #region POST endpoints
        /// <summary>
        /// Add a anime to a database.
        /// </summary>
        /// <returns>Validation string for record creation.</returns>
        [HttpPost()]
        [ProducesResponseType(typeof(MultiResults), 200)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> AddAnime(AnimePostModel model)
        {
            try
            {
                // TODO: Once I add endpoint to get Studios, need validation on the studio id. Also need to validate zeros
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

                    // Validate result and send correct response
                    if (result.FirstOrDefault().Key.Contains(ResponseStatus.Success.ToString()))
                    {
                        // Append results
                        var finalResults = new MultiResults()
                        {
                            SuccessfulRecordsCount = 1,
                            FailureRecordsCount = 0,
                            ResultList = result
                        };
                        return Ok(finalResults);
                    } else
                    {
                        // Append results
                        var finalResults = new MultiResults()
                        {
                            SuccessfulRecordsCount = 0,
                            FailureRecordsCount = 1,
                            ResultList = result
                        };
                        return BadRequest("Error creating anime. Please validate and try again.");
                    }
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

        /// <summary>
        /// Add multiple anime to a database.
        /// </summary>
        /// <returns>Validation string for each record creation.</returns>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(MultiResults), 200)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> AddAnimeBulk(List<AnimePostModel> animeList)
        {
            try
            {
                // TODO: Once I add endpoint to get Studios, need validation on the studio id. Also need to validate zeros
                //Validation.
                var validationResults = new Dictionary<string, string>();
                int successCount = 0;
                int faliureCount = 0;

                if (animeList == null)
                {
                    return BadRequest("Model cannot be null or empty.");
                }

                foreach (var anime in animeList)
                {
                    // Make sure record doesn't already exist. If it does we remove from the request object and add flag to results.
                    var checkName = await _animeService.GetAnimeAsync(anime.AnimeName.ToLower());
                    if (string.IsNullOrEmpty(checkName.AnimeName))
                    {
                        continue;
                    }
                    else
                    {
                        animeList.Remove(anime);
                        validationResults.Add(ResponseStatus.Failure.ToString() + ":" + anime.AnimeName,  "Duplicate record detected. Record not created for: " + anime.AnimeName + ".");
                        continue;
                    }
                }

                // Add to db.
                var result = await _animeService.AddAnimeBulkAsync(animeList);

                // Add earlier validation to updated list
                foreach (var validation in validationResults)
                {
                    result.Add(validation.Key, validation.Value);
                }

                // Get status count for records.
                foreach(var record in result)
                {
                    if (record.Key.Contains(ResponseStatus.Success.ToString()))
                    {
                        successCount++;
                    }
                    if (record.Key.Contains(ResponseStatus.Success.ToString()))
                    {
                        faliureCount++;
                    }
                }

                // Append results
                var  finalResults = new MultiResults()
                {
                    SuccessfulRecordsCount = successCount,
                    FailureRecordsCount = faliureCount,
                    ResultList = result
                };

                if (successCount == 0 && faliureCount > 0)
                {
                    return BadRequest(finalResults);
                } else
                {
                    return Ok(finalResults);
                }
            }
            catch (Exception ex)
            {
                var exceptionErrorModel = new ExceptionErrorModel(ex.Message, ex.StackTrace);
                return BadRequest(exceptionErrorModel);
            }
        }
        #endregion

        #region PUT endpoints
        /// <summary>
        /// Update a anime in the database.
        /// </summary>
        /// <returns>Validation string for record update.</returns>
        [HttpPut()]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> UpdateAnime(AnimePutModel model)
        {
            try
            {
                // TODO: Once I add endpoint to get Studios, need validation on the studio id. Also need to validate zeros
                //Validation.
                if (model == null)
                {
                    return BadRequest("Model cannot be null or empty.");
                }

                // Make sure record doesn't already exist
                var checkName = await _animeService.GetAnimeAsync(model.AnimeName.ToLower());
                if (string.IsNullOrEmpty(checkName.AnimeName) || model.AnimeId != checkName.AnimeId)
                {
                    // Add to db.
                    var result = await _animeService.UpdateAnimeAsync(model);

                    // Validate result and send correct response
                    if (result == ResponseStatus.Success.ToString())
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest("Error updating anime. Please validate and try again.");
                    }
                }
                else
                {
                    return NotFound(model.AnimeName + " doesn't exists. Please validate and try again.");
                }
            }
            catch (Exception ex)
            {
                var exceptionErrorModel = new ExceptionErrorModel(ex.Message, ex.StackTrace);
                return BadRequest(exceptionErrorModel);
            }
        }
        #endregion

        #region DELETE endpoints
        /// <summary>
        /// Soft delete a anime in the database.
        /// </summary>
        /// <returns>Validation string for record deletion.</returns>
        [HttpDelete("{animeName}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ExceptionErrorModel), 400)]
        public async Task<ActionResult> DeleteAnime(string animeName)
        {
            try
            {
                // Validate param
                if (string.IsNullOrEmpty(animeName))
                {
                    return BadRequest("Anime name cannot be null or empty.");
                }

                // Converting to lower case to remove any future case senstive issues.
                var convertedName = animeName.ToLower();

                // Validate record exists result.
                var getResult = await _animeService.GetAnimeAsync(convertedName);

                // Flag for missing result
                if (string.IsNullOrEmpty(getResult.AnimeName))
                {
                    return NotFound($"{animeName} doesn't exist in the database. Please validate and try again.");
                }

                // Get result.
                var result = await _animeService.DeleteAnimeAsync(convertedName);

                // Results
                return Ok(result);
            }
            catch (Exception ex)
            {
                var exceptionErrorModel = new ExceptionErrorModel(ex.Message, ex.StackTrace);
                return BadRequest(exceptionErrorModel);
            }
        }
        #endregion
    }
}
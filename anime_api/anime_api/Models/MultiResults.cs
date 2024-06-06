using System.ComponentModel.DataAnnotations;

namespace anime_api.Models
{
    /// <summary>
    /// Represents the result of an operation involving multiple records.
    /// </summary>
    public class MultiResults
    {
        /// <summary>
        /// Gets or sets the count of successfully processed records.
        /// </summary>
        public int SuccessfulRecordsCount { get; set; }

        /// <summary>
        /// Gets or sets the count of failed records.
        /// </summary>
        public int FailureRecordsCount { get; set; }

        /// <summary>
        /// Gets or sets the list of results with their statuses.
        /// </summary>
        [Required]
        public Dictionary<string, string> ResultList { get; set; } = new();
    }
}
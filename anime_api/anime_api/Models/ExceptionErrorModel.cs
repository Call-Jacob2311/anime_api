namespace anime_api.Models
{
    /// <summary>
    /// Represents an error model for exceptions.
    /// </summary>
    public sealed class ExceptionErrorModel
    {
        /// <summary>
        /// Gets the exception message.
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// Gets the stack trace of the exception.
        /// </summary>
        public string? StackTrace { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionErrorModel"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="stackTrace">The stack trace of the exception.</param>
        public ExceptionErrorModel(string? message, string? stackTrace)
        {
            Message = message;
            StackTrace = stackTrace;
        }
    }
}

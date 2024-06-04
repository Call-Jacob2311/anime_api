namespace anime_api.Models
{
    public class MultiResults
    {
        public int SuccessfulRecordsCount {  get; set; }
        public int FailureRecordsCount { get; set; }
        public required Dictionary<string, string> ResultList { get; set; }
    }
}

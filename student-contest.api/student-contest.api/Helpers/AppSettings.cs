namespace student_contest.api.Helpers
{
    public class AppSettings
    {
        public string AccessTokenSecret { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
    }
}

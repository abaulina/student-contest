namespace StudentContest.Api.Models
{
    public class UserTokenSet
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}

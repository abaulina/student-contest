using Newtonsoft.Json;

namespace StudentContest.Api.Models
{
    public class AuthenticatedResponse
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string? RefreshToken { get; set; }

        [JsonConstructor]
        public AuthenticatedResponse(int id, string token, string? refreshToken)
        {
            Id = id;
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}

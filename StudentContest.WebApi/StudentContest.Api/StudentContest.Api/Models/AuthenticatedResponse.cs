using Newtonsoft.Json;

namespace StudentContest.Api.Models
{
    public class AuthenticatedResponse
    { 
        public string Token { get; set; }
        public string? RefreshToken { get; set; }

        [JsonConstructor]
        public AuthenticatedResponse(string token, string? refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}

using Newtonsoft.Json;

namespace StudentContest.Api.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }

        public AuthenticateResponse(User user, string token, string? refreshToken)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email= user.Email;
            Token = token;
            RefreshToken = refreshToken;
        }

        [JsonConstructor]
        public AuthenticateResponse(int id, string firstName, string lastName, string email, string token, string? refreshToken)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}

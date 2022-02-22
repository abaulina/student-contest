using Newtonsoft.Json;

namespace StudentContest.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }

        public User(){}

        public User(RegisterRequest registerRequest)
        {
            Email = registerRequest.Email;
            FirstName = registerRequest.FirstName;
            LastName = registerRequest.LastName;
            PasswordHash = registerRequest.Password;
        }
    }
}

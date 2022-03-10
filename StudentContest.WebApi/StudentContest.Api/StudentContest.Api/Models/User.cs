using Microsoft.AspNetCore.Identity;

namespace StudentContest.Api.Models
{
    public class User: IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

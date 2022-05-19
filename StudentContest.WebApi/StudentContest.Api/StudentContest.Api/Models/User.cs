using Microsoft.AspNetCore.Identity;

namespace StudentContest.Api.Models
{
    public class User: IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<UserNote> UserNotes { get; set; }
    }
}

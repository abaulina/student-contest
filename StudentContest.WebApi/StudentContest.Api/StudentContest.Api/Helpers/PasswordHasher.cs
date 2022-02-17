using BCryptNet = BCrypt.Net.BCrypt;

namespace StudentContest.Api.Helpers
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCryptNet.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCryptNet.Verify(password, hash);
        }
    }
}

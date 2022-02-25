using StudentContest.Api.Models;

namespace StudentContest.Api.Services.UserRepository
{
    public interface IUserRepository
    {
        Task Add(User user);
        Task<User?> GetByEmail(string email);
        Task<User?> Find(int id);
    }
}

using StudentContest.Api.Models;

namespace StudentContest.Api.Services.RefreshTokenRepository
{
    public interface ITokenRepository
    {
        Task<UserTokenSet?> GetByAccessToken(string token);
        Task<UserTokenSet?> GetByRefreshToken(string token);

        Task Create(UserTokenSet userTokenSet);

        Task Delete(int id);

        Task DeleteAll(int userId);
    }
}

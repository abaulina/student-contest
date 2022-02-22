using StudentContest.Api.Models;

namespace StudentContest.Api.Services.RefreshTokenRepository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByToken(string token);

        Task Create(RefreshToken refreshToken);

        Task Delete(int id);

        Task DeleteAll(int userId);
    }
}

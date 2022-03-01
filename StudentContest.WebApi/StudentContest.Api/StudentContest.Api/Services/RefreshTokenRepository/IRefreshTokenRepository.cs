using StudentContest.Api.Models;

namespace StudentContest.Api.Services.RefreshTokenRepository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByRefreshToken(string token);

        Task Create(RefreshToken userTokenSet);

        Task Delete(int id);

        Task DeleteAll(int userId);
    }
}

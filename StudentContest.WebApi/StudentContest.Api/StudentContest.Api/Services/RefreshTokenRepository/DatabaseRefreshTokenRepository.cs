using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;

namespace StudentContest.Api.Services.RefreshTokenRepository
{
    public class DatabaseRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationContext _applicationContext;

        public DatabaseRefreshTokenRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }
        
        public async Task<RefreshToken?> GetByRefreshToken(string token)
        {
            return await _applicationContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task Create(RefreshToken userTokenSet)
        {
            _applicationContext.RefreshTokens.Add(userTokenSet);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var token = await _applicationContext.RefreshTokens.FindAsync(id);
            if (token != null)
            {
                _applicationContext.Remove(token);
                await _applicationContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAll(int userId)
        {
           var refreshTokens = await _applicationContext.RefreshTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            _applicationContext.RefreshTokens.RemoveRange(refreshTokens);
            await _applicationContext.SaveChangesAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;

namespace StudentContest.Api.Services.RefreshTokenRepository
{
    public class DatabaseRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthenticationContext _authenticationContext;

        public DatabaseRefreshTokenRepository(AuthenticationContext authenticationContext)
        {
            _authenticationContext = authenticationContext;
        }
        
        public async Task<RefreshToken?> GetByRefreshToken(string token)
        {
            return await _authenticationContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task Create(RefreshToken userTokenSet)
        {
            _authenticationContext.RefreshTokens.Add(userTokenSet);
            await _authenticationContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var token = await _authenticationContext.RefreshTokens.FindAsync(id);
            if (token != null)
            {
                _authenticationContext.Remove(token);
                await _authenticationContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAll(int userId)
        {
           var refreshTokens = await _authenticationContext.RefreshTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            _authenticationContext.RefreshTokens.RemoveRange(refreshTokens);
            await _authenticationContext.SaveChangesAsync();
        }
    }
}

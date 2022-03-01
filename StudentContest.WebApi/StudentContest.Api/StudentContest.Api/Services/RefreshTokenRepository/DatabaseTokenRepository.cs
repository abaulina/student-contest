using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Models;

namespace StudentContest.Api.Services.RefreshTokenRepository
{
    public class DatabaseTokenRepository : ITokenRepository
    {
        private readonly AuthenticationContext _authenticationContext;

        public DatabaseTokenRepository(AuthenticationContext authenticationContext)
        {
            _authenticationContext = authenticationContext;
        }

        public async Task<UserTokenSet?> GetByAccessToken(string token)
        {
            return await _authenticationContext.Tokens.FirstOrDefaultAsync(x => x.AccessToken == token);
        }

        public async Task<UserTokenSet?> GetByRefreshToken(string token)
        {
            return await _authenticationContext.Tokens.FirstOrDefaultAsync(x => x.RefreshToken == token);
        }

        public async Task Create(UserTokenSet userTokenSet)
        {
            _authenticationContext.Tokens.Add(userTokenSet);
            await _authenticationContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var token = await _authenticationContext.Tokens.FindAsync(id);
            if (token != null)
            {
                _authenticationContext.Remove(token);
                await _authenticationContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAll(int userId)
        {
           var refreshTokens = await _authenticationContext.Tokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            _authenticationContext.Tokens.RemoveRange(refreshTokens);
            await _authenticationContext.SaveChangesAsync();
        }
    }
}

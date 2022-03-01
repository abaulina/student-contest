using StudentContest.Api.Models;
using StudentContest.Api.Services.RefreshTokenRepository;

namespace StudentContest.Api.Auth
{
    public class Authenticator
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public Authenticator(ITokenGenerator tokenGenerator,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _tokenGenerator = tokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthenticatedResponse> Authenticate(User user)
        {
            var accessToken = _tokenGenerator.GenerateJwtToken(user);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            var refreshTokenDto = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id
            };

            await _refreshTokenRepository.Create(refreshTokenDto);

            return new AuthenticatedResponse( accessToken, refreshToken);
        }
    }
}

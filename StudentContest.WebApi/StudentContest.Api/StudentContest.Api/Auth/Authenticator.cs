using StudentContest.Api.Models;
using StudentContest.Api.Services.RefreshTokenRepository;

namespace StudentContest.Api.Auth
{
    public class Authenticator
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ITokenRepository _tokenRepository;

        public Authenticator(ITokenGenerator tokenGenerator,
            ITokenRepository tokenRepository)
        {
            _tokenGenerator = tokenGenerator;
            _tokenRepository = tokenRepository;
        }

        public async Task<AuthenticatedResponse> Authenticate(User user)
        {
            var accessToken = _tokenGenerator.GenerateJwtToken(user);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            var refreshTokenDto = new UserTokenSet
            {
                RefreshToken = refreshToken,
                UserId = user.Id,
                AccessToken = accessToken
            };

            await _tokenRepository.Create(refreshTokenDto);

            return new AuthenticatedResponse( accessToken, refreshToken);
        }
    }
}

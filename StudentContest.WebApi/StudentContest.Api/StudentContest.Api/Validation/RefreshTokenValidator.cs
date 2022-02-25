using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StudentContest.Api.Models;

namespace StudentContest.Api.Validation
{
    public class RefreshTokenValidator
    {
        private readonly AuthenticationConfiguration _configuration;

        public RefreshTokenValidator(AuthenticationConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Validate(string refreshToken)
        {
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.RefreshTokenSecret)),
                ValidIssuer = _configuration.Issuer,
                ValidAudience = _configuration.Audience,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(refreshToken, validationParameters, out _);
        }
    }
}

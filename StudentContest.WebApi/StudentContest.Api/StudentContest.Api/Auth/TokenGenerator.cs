using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StudentContest.Api.Models;

namespace StudentContest.Api.Auth
{
    public interface ITokenGenerator
    {
        public string GenerateJwtToken(User user);
        public string GenerateRefreshToken();
    }
    
    public class TokenGenerator : ITokenGenerator
    {
        private readonly AuthenticationConfiguration _authenticationConfiguration;

        public TokenGenerator(AuthenticationConfiguration authenticationConfiguration)
        {
            _authenticationConfiguration = authenticationConfiguration;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new("id", user.Id)
            };

            var expirationTime = DateTime.UtcNow.AddMinutes(_authenticationConfiguration.AccessTokenExpirationMinutes);
            return GenerateToken(
                _authenticationConfiguration.AccessTokenSecret,
                _authenticationConfiguration.Issuer,
                _authenticationConfiguration.Audience,
                expirationTime,
                claims);
        }

        public string GenerateRefreshToken()
        {
            var expirationTime = DateTime.UtcNow.AddMinutes(_authenticationConfiguration.RefreshTokenExpirationDays);

            return GenerateToken(
                _authenticationConfiguration.RefreshTokenSecret,
                _authenticationConfiguration.Issuer,
                _authenticationConfiguration.Audience,
                expirationTime);
        }

        private string GenerateToken(string secretKey, string issuer, string audience, DateTime utcExpirationTime,
            IEnumerable<Claim>? claims = null)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                DateTime.UtcNow,
                utcExpirationTime,
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

using System.Security.Authentication;
using StudentContest.Api.Auth;
using StudentContest.Api.ExceptionMiddleware;
using StudentContest.Api.Models;
using StudentContest.Api.Services.RefreshTokenRepository;
using StudentContest.Api.Validation;

namespace StudentContest.Api.Services
{
    public interface IAuthService
    {
        Task<AuthenticatedResponse> Login(LoginRequest loginRequest);
        Task Logout(string refreshToken);
        Task<AuthenticatedResponse> RefreshToken(string refreshToken);
    }

    public class AuthService:IAuthService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly RefreshTokenValidator _refreshRefreshTokenValidator;
        private readonly Authenticator _authenticator;
        private readonly IUserManagerWrapper _userManagerWrapper;

        public AuthService(IRefreshTokenRepository refreshTokenRepository, RefreshTokenValidator refreshRefreshTokenValidator, Authenticator authenticator, IUserManagerWrapper userManagerWrapper)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _refreshRefreshTokenValidator = refreshRefreshTokenValidator;
            _authenticator = authenticator;
            _userManagerWrapper = userManagerWrapper;
        }

        public async Task<AuthenticatedResponse> Login(LoginRequest loginRequest)
        {
            var user = await _userManagerWrapper.FindByEmailAsync(loginRequest.Email);

            var isPasswordCorrect = await _userManagerWrapper.CheckPasswordAsync(user, loginRequest.Password);
            if (user == null || !isPasswordCorrect)
                throw new InvalidCredentialException("Email or password is incorrect");

            var userRoles = await _userManagerWrapper.GetUserRolesAsync(user.Id);
            var response = await _authenticator.Authenticate(user, userRoles);
            return response;
        }

        public async Task Logout(string token)
        {
            var refreshToken = await GetValidRefreshToken(token);

            await _refreshTokenRepository.DeleteAll(refreshToken.UserId);
        }

        public async Task<AuthenticatedResponse> RefreshToken(string token)
        {
            var refreshToken = await GetValidRefreshToken(token);

            await _refreshTokenRepository.Delete(refreshToken.Id);

            var user = await _userManagerWrapper.GetUserInfoAsync(refreshToken.UserId);
            var userRoles = await _userManagerWrapper.GetUserRolesAsync(user.Id);

            var response = await _authenticator.Authenticate(user, userRoles);
            return response;
        }

        private async Task<RefreshToken> GetValidRefreshToken(string inputToken)
        {
            _refreshRefreshTokenValidator.Validate(inputToken);
            var refreshToken = await _refreshTokenRepository.GetByRefreshToken(inputToken);

            if (refreshToken == null)
                throw new ApiException("Invalid refresh token");
            return refreshToken;
        }
    }
}

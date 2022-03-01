using System.Security.Authentication;
using StudentContest.Api.Auth;
using StudentContest.Api.Helpers;
using StudentContest.Api.Models;
using StudentContest.Api.Services.RefreshTokenRepository;
using StudentContest.Api.Services.UserRepository;
using StudentContest.Api.Validation;

namespace StudentContest.Api.Services
{
    public interface IUserService
    {
        Task<AuthenticatedResponse> Login(LoginRequest loginRequest);
        Task Logout(int userId);
        Task<User?> GetUserInfo(string token);
        Task Register(RegisterRequest registerRequest);
        Task<AuthenticatedResponse> RefreshToken (string refreshToken);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRegisterRequestValidator _registerRequestValidator;
        private readonly ITokenRepository _tokenRepository;
        private readonly RefreshTokenValidator _refreshRefreshTokenValidator;
        private readonly Authenticator _authenticator;

        public UserService(IRegisterRequestValidator registerRequestValidator, IPasswordHasher passwordHasher, RefreshTokenValidator refreshRefreshTokenValidator, ITokenRepository tokenRepository, Authenticator authenticator, IUserRepository userRepository)
        {
            _registerRequestValidator = registerRequestValidator;
            _passwordHasher = passwordHasher;
            _refreshRefreshTokenValidator = refreshRefreshTokenValidator;
            _tokenRepository = tokenRepository;
            _authenticator = authenticator;
            _userRepository = userRepository;
        }

        public async Task<AuthenticatedResponse> Login(LoginRequest loginRequest)
        {
            var user = await  _userRepository.GetByEmail(loginRequest.Email);

            if (user == null || !_passwordHasher.VerifyPassword( loginRequest.Password, user.PasswordHash))
                throw new InvalidCredentialException("Username or password is incorrect");
            
            var response = await _authenticator.Authenticate(user);
            return response;
        }

        public async Task Logout(int userId)
        {
            await _tokenRepository.DeleteAll(userId);
        }

        public async Task<User?> GetUserInfo(string token)
        {
            var refreshToken = await _tokenRepository.GetByAccessToken(token);
            if (refreshToken == null)
                throw new KeyNotFoundException("Invalid refresh token");
            return await GetUser(refreshToken.UserId);
        }

        public async Task Register(RegisterRequest registerRequest)
        {
            _registerRequestValidator.ValidateRequestData(registerRequest);

            var newUser = new User(registerRequest);
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser.PasswordHash);

            await _userRepository.Add(newUser);
        }

        public async Task<AuthenticatedResponse> RefreshToken(string token)
        {
            _refreshRefreshTokenValidator.Validate(token);
            var refreshToken = await _tokenRepository.GetByRefreshToken(token);
            
            if (refreshToken==null)
                throw new KeyNotFoundException("Invalid refresh token");

            await _tokenRepository.Delete(refreshToken.Id);

            var user = await GetUser(refreshToken.UserId);
            
            var response = await _authenticator.Authenticate(user);
            return response;
        }

        private async Task<User?> GetUser(int id)
        {
            return await _userRepository.Find(id);
        }
    }
}

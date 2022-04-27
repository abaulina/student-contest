using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Auth;
using StudentContest.Api.ExceptionMiddleware;
using StudentContest.Api.Models;
using StudentContest.Api.Services.RefreshTokenRepository;
using StudentContest.Api.Validation;

namespace StudentContest.Api.Services
{
    public interface IUserService
    {
        Task<AuthenticatedResponse> Login(LoginRequest loginRequest);
        Task Logout(string refreshToken);
        Task<User?> GetUserInfo(int userId);
        Task Register(RegisterRequest registerRequest, string role = "User");
        Task<AuthenticatedResponse> RefreshToken (string refreshToken);
        Task<IEnumerable<User>> GetUsers();
    }

    public class UserService : IUserService
    {
        private readonly IUserManagerWrapper _userManagerWrapper;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly RefreshTokenValidator _refreshRefreshTokenValidator;
        private readonly IRegisterRequestValidator _registerRequestValidator;
        private readonly Authenticator _authenticator;

        public UserService(RefreshTokenValidator refreshRefreshTokenValidator, IRefreshTokenRepository refreshTokenRepository, Authenticator authenticator, IUserManagerWrapper userManagerWrapper, IRegisterRequestValidator registerRequestValidator)
        {
            _refreshRefreshTokenValidator = refreshRefreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;
            _authenticator = authenticator;
            _userManagerWrapper = userManagerWrapper;
            _registerRequestValidator = registerRequestValidator;
        }

        public async Task<AuthenticatedResponse> Login(LoginRequest loginRequest)
        {
            var user = await  _userManagerWrapper.FindByEmailAsync(loginRequest.Email);

            var isPasswordCorrect = await _userManagerWrapper.CheckPasswordAsync(user, loginRequest.Password);
            if (user == null || !isPasswordCorrect)
                throw new InvalidCredentialException("Email or password is incorrect");

            var userRoles = await _userManagerWrapper.GetRolesAsync(user);
            var response = await _authenticator.Authenticate(user, userRoles);
            return response;
        }

        public async Task Logout(string token)
        {
            var refreshToken = await GetValidRefreshToken(token);

            await _refreshTokenRepository.DeleteAll(refreshToken.UserId);
        }

        public async Task<User?> GetUserInfo(int userId)
        {
            return await GetUser(userId);
        }

        public async Task Register(RegisterRequest registerRequest, string role = "User")
        {
            _registerRequestValidator.ValidateUserPersonalData(registerRequest);
            var newUser = new User {Email = registerRequest.Email, FirstName = registerRequest.FirstName, LastName = registerRequest.LastName, UserName  = registerRequest.Email};
            var result = await _userManagerWrapper.CreateAsync(newUser, registerRequest.Password);

            if (!result.Succeeded)
            {
                var primaryError = result.Errors.FirstOrDefault();
                switch (primaryError?.Code)
                {
                    case nameof(IdentityErrorDescriber.DuplicateEmail):
                    case nameof(IdentityErrorDescriber.InvalidEmail):
                    case nameof(IdentityErrorDescriber.DuplicateUserName):
                        throw new ApiException("Email is invalid");
                    case nameof(IdentityErrorDescriber.PasswordTooShort):
                        throw new ApiException("Password is invalid. It must be at least 8 characters");
                }

                throw new DbUpdateException();
            }

            await _userManagerWrapper.AddToRoleAsync(newUser, role);
        }

        public async Task<AuthenticatedResponse> RefreshToken(string token)
        {
            var refreshToken = await GetValidRefreshToken(token);

            await _refreshTokenRepository.Delete(refreshToken.Id);

            var user = await GetUser(refreshToken.UserId);
            var userRoles = await _userManagerWrapper.GetRolesAsync(user);

            var response = await _authenticator.Authenticate(user, userRoles);
            return response;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userManagerWrapper.FindAllAsync();
        }

        private async Task<User?> GetUser(int id)
        {
            var user = await _userManagerWrapper.FindByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException();
            return new User
            {
                Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, Id = user.Id
            };
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

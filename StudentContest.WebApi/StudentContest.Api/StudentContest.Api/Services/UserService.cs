using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Authorization;
using StudentContest.Api.Helpers;
using StudentContest.Api.Models;
using StudentContest.Api.Services.RefreshTokenRepository;
using StudentContest.Api.Validation;

namespace StudentContest.Api.Services
{
    public interface IUserService
    {
        Task<AuthenticatedResponse> Login(LoginRequest loginRequest);
        Task Logout(int userId);
        Task<User?> GetUserInfo(int id);
        Task Register(RegisterRequest registerRequest);
        Task<AuthenticatedResponse> RefreshToken (string refreshToken);
    }

    public class UserService : IUserService
    {
        private readonly AuthenticationContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRegisterRequestValidator _registerRequestValidator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly Authenticator _authenticator;

        public UserService(AuthenticationContext context, IRegisterRequestValidator registerRequestValidator, IPasswordHasher passwordHasher, RefreshTokenValidator refreshTokenValidator, IRefreshTokenRepository refreshTokenRepository, Authenticator authenticator)
        {
            _context = context;
            _registerRequestValidator = registerRequestValidator;
            _passwordHasher = passwordHasher;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;
            _authenticator = authenticator;
        }

        public async Task<AuthenticatedResponse> Login(LoginRequest loginRequest)
        {
            var user = await  _context.Users.FirstOrDefaultAsync(x => x.Email == loginRequest.Email);

            if (user == null || !_passwordHasher.VerifyPassword( loginRequest.Password, user.PasswordHash))
                throw new InvalidCredentialException("Username or password is incorrect");
            
            var response = await _authenticator.Authenticate(user);
            return response;
        }

        public async Task Logout(int userId)
        {
            await _refreshTokenRepository.DeleteAll(userId);
        }

        public async Task<User?> GetUserInfo(int id)
        {
            return await GetUser(id);
        }

        public async Task Register(RegisterRequest registerRequest)
        {
            _registerRequestValidator.ValidateRequestData(registerRequest);

            var newUser = new User(registerRequest);
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser.PasswordHash);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }

        public async Task<AuthenticatedResponse> RefreshToken(string token)
        {
            _refreshTokenValidator.Validate(token);
            var refreshToken = await _refreshTokenRepository.GetByToken(token);
            
            if (refreshToken==null)
                throw new KeyNotFoundException("Invalid refresh token");

            await _refreshTokenRepository.Delete(refreshToken.Id);

            var user = await GetUser(refreshToken.UserId);

            if (user == null)
                throw new ArgumentException("User not found");
            
            var response = await _authenticator.Authenticate(user);
            return response;
        }

        private async Task<User?> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }
    }
}

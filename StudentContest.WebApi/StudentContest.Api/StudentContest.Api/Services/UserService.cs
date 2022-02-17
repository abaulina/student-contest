using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using StudentContest.Api.Authorization;
using StudentContest.Api.Helpers;
using StudentContest.Api.Models;
using StudentContest.Api.Validation;

namespace StudentContest.Api.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Login(LoginRequest loginRequest, string ipAddress);
        Task Logout(string refreshToken, string ipAddress);
        Task<User?> GetCurrentUserInfo(int id);
        Task Register(RegisterRequest registerRequest);
        Task<AuthenticateResponse> RefreshToken (string refreshToken, string ipAddress);
    }

    public class UserService : IUserService
    {
        private readonly UserContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtUtils _jwtUtils;
        private readonly IRegisterRequestValidator _registerRequestValidator;

        public UserService(UserContext context, IJwtUtils jwtUtils, IRegisterRequestValidator registerRequestValidator, IPasswordHasher passwordHasher)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _registerRequestValidator = registerRequestValidator;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthenticateResponse> Login(LoginRequest loginRequest, string ipAddress)
        {
            var user = await  _context.Users.FirstOrDefaultAsync(x => x.Email == loginRequest.Email);

            if (user == null || !_passwordHasher.VerifyPassword( loginRequest.Password, user.PasswordHash))
                throw new InvalidCredentialException("Username or password is incorrect");
            
            var jwtToken = _jwtUtils.GenerateToken(user);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);

            _jwtUtils.RemoveOldRefreshTokens(user);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public async Task Logout(string token, string ipAddress)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is required");
            
            var user = await GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new ArgumentException("Invalid token");

            _jwtUtils.RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement due to logout", null);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetCurrentUserInfo(int id)
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

        public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                _jwtUtils.RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            if (!refreshToken.IsActive)
                throw new ArgumentException("Invalid refresh token");

            var newRefreshToken = _jwtUtils.RotateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            _jwtUtils.RemoveOldRefreshTokens(user);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new AuthenticateResponse(user, _jwtUtils.GenerateToken(user), newRefreshToken.Token);
        }

        private async Task<User?> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");
            return user;
        }

        private async Task<User> GetUserByRefreshToken(string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                throw new ArgumentException("Invalid refresh token");

            return user;
        }
    }
}

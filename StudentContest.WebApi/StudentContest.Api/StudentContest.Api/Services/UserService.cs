using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentContest.Api.ExceptionMiddleware;
using StudentContest.Api.Models;
using StudentContest.Api.Validation;

namespace StudentContest.Api.Services
{
    public interface IUserService
    {
        Task<User?> GetUserInfo(int userId);
        Task<User> Register(RegisterRequest registerRequest, string role = "User");
        Task<IEnumerable<User>> GetUsers();
    }

    public class UserService : IUserService
    {
        private readonly IUserManagerWrapper _userManagerWrapper;
        private readonly IRegisterRequestValidator _registerRequestValidator;

        public UserService(IUserManagerWrapper userManagerWrapper, IRegisterRequestValidator registerRequestValidator)
        {
            _userManagerWrapper = userManagerWrapper;
            _registerRequestValidator = registerRequestValidator;
        }

        public async Task<User?> GetUserInfo(int userId)
        {
            return await _userManagerWrapper.GetUserInfoAsync(userId);
        }

        public async Task<User> Register(RegisterRequest registerRequest, string role = "User")
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

            var addedUser = await _userManagerWrapper.FindByEmailAsync(newUser.Email);
            await _userManagerWrapper.AddToRoleAsync(addedUser.Id, role);
            return addedUser;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userManagerWrapper.GetUsersAsync();
        }
    }
}

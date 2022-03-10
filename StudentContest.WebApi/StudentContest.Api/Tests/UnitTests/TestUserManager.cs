using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;
using StudentContest.Api.Models;

namespace StudentContest.Api.Tests.UnitTests
{
    internal class TestUserManager
    {
        public static UserManager<User> CreateUserManager(AuthenticationContext context)
        {
            var idOptions = new IdentityOptions
            {
                Password =
                {
                    RequiredLength = 8
                },
                User = {RequireUniqueEmail = true}
            };

            var store = new UserStore<User>(context);
            var userManager = new Mock<UserManager<User>>(store, null, null, null, null, null, null, null, null);
            userManager.Object.UserValidators.Add(new UserValidator<User>());
            userManager.Object.PasswordValidators.Add(new PasswordValidator<User>());
            userManager.Object.Options = idOptions;

            userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))!.ReturnsAsync((string id) => context.Users.Find(id));
            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).Callback<User, string>((x, y) =>
            {
                x.Id = (context.Users.Count() + 1).ToString();
                x.PasswordHash = y;
                context.Add(x);
                context.SaveChanges();
            }).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))!
                .ReturnsAsync((string email) => context.Users.FirstOrDefault(x => x.Email == email));
            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User?>(), It.IsAny<string>()))!
                .ReturnsAsync((User user, string pass) => user!=null && user.PasswordHash == pass);

            return userManager.Object;
        }
    }
}

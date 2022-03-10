using System;
using StudentContest.Api.Models;
using StudentContest.Api.Validation;
using Xunit;

namespace StudentContest.Api.Tests.UnitTests
{
    public class RegisterRequestValidatorTests
    {
        private readonly RegisterRequestValidator _registerRequestValidator;

        public RegisterRequestValidatorTests()
        {
            _registerRequestValidator = new RegisterRequestValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData(".")]
        [InlineData("/User")]
        [InlineData("User?")]
        [InlineData("Use.r")]
        [InlineData("1User")]
        [InlineData("User1")]
        public void ValidateLastName_InvalidLastName_ThrowsException(string lastName)
        {
            Assert.Throws<ArgumentException>(() => _registerRequestValidator.ValidateLastName(lastName));
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData(".")]
        [InlineData("/User")]
        [InlineData("User?")]
        [InlineData("Use.r")]
        [InlineData("1User")]
        [InlineData("User1")]
        public void ValidateFirstName_InvalidFirstName_ThrowsException(string firstName)
        {
            Assert.Throws<ArgumentException>(() => _registerRequestValidator.ValidateFirstName(firstName));
        }

        [Theory]
        [InlineData("")]
        [InlineData("user@example.c")]
        [InlineData("userexample.com")]
        [InlineData(".user@example.com")]
        [InlineData("user.@example.com")]
        [InlineData("user@.com")]
        public void ValidateEmail_IncorrectFormat_ThrowsException(string email)
        {
            Assert.Throws<ArgumentException>(() => _registerRequestValidator.ValidateEmail(email));
        }

        [Fact]
        public void ValidateRequestData_Success()
        {
            var registerRequest = new RegisterRequest
            {
                Email = "newUser@example.com",
                FirstName = "New",
                LastName = "User",
                Password = "12345678"
            };

            _registerRequestValidator.ValidateUserPersonalData(registerRequest);
        }
    }
}

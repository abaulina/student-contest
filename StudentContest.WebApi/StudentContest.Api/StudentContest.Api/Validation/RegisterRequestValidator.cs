using System.Text.RegularExpressions;
using StudentContest.Api.Models;

namespace StudentContest.Api.Validation
{
    public interface IRegisterRequestValidator
    {
        public void ValidateUserPersonalData(RegisterRequest registerRequest);
    }

    public class RegisterRequestValidator : IRegisterRequestValidator
    {
        public void ValidateUserPersonalData(RegisterRequest registerRequest)
        {
            ValidateFirstName(registerRequest.FirstName); 
            ValidateLastName(registerRequest.LastName);
            ValidateEmail(registerRequest.Email);
        }

        public void ValidateFirstName(string? firstName)
        {
            var regex = new Regex(@"^[a-zA-ZаЯЁёА-я]+([-]?\s?[a-zA-ZЁёА-я])?$");
            if (firstName == null || !regex.IsMatch(firstName))
                throw new ArgumentException("First name is invalid");
        }

        public void ValidateLastName(string? lastName)
        {
            var regex = new Regex(@"^[a-zA-ZаЯЁёА-я]+([-]?\s?[a-zA-ZЁёА-я])?$");
            if (lastName == null || !regex.IsMatch(lastName))
                throw new ArgumentException("Last name is invalid");
        }

        public void ValidateEmail(string? email)
        {
            var regex = new Regex(
                @"^(([^<>()[\].,;:\s@""]+(\.[^<>()[\].,;:\s@""]+)*)|("".+""))@(([^<>()[\].,;:\s@""]+\.)+[^<>()[\].,;:\s@""]{2,})$");
            if (email == null || !regex.IsMatch(email))
                throw new ArgumentException("Email is invalid");
        }
    }
}

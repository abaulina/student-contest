using System.Text.RegularExpressions;
using student_contest.api.Models;

namespace student_contest.api.Validation
{
    public interface IRegisterRequestValidator
    {
        public void ValidateRequestData(RegisterRequest registerRequest);
    }

    public class RegisterRequestValidator : IRegisterRequestValidator
    {
        private readonly UserContext _context;

        public RegisterRequestValidator(UserContext context)
        {
            _context = context;
        }

        public void ValidateRequestData(RegisterRequest registerRequest)
        {
            ValidateEmail(registerRequest.Email);
            ValidatePassword(registerRequest.Password);
            ValidateFirstName(registerRequest.FirstName); 
            ValidateLastName(registerRequest.LastName);
        }

        public void ValidateEmail(string email)
        {
            IsCorrectEmailFormat(email);
            IsEmailBeingUsed(email);
        }

        private void IsCorrectEmailFormat(string? email)
        {
            var regex = new Regex(
                @"?i^(([^<>()[\].,;:\s@""]+(\.[^<>()[\].,;:\s@""]+)*)|("".+""))@(([^<>()[\].,;:\s@""]+\.)+[^<>()[\].,;:\s@""]{2,})$");
            if (email == null || !regex.IsMatch(email))
                throw new ArgumentException("Email is invalid");
        }

        private void IsEmailBeingUsed(string email)
        {
            if (_context.Users.Any(x => x.Email == email))
                throw new ArgumentException("This email is already being used");
        }

        public void ValidatePassword(string? password)
        {
            if (password is {Length: < 8})
                throw new ArgumentException("Password is invalid. It must be at least 8 characters");
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
    }
}

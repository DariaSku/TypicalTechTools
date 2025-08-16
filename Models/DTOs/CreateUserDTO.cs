using System.ComponentModel.DataAnnotations;

namespace TypicalTechTools.Models.DTOs
{
    public class CreateUserDTO
    {
        public string UserName { get; set; } = string.Empty;
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[*.!@$%^&,?_-])[A-Za-z0-9*.!@$%^&,?_-].{8,12}$",
            ErrorMessage = "Your password must have at least 1 upper case letter, 1 lower case letter, 1 number" +
                           "1 symbol and be between 8 -12 characters")]
        public string Password { get; set; } = string.Empty;
        public string PasswordConfirmation { get; set; } = string.Empty;
    }
}

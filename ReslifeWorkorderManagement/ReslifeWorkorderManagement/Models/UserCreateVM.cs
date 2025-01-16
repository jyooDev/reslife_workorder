using System.ComponentModel.DataAnnotations;

namespace ReslifeWorkorderManagement.Models
{
    public class UserCreateVM
    {

        [Required(ErrorMessage = "First name is required.")]
        public required string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public required string? LastName { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public required string? Email { get; set; } 

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public required string? Password { get; set; }

        public required string Role { get; set; }
    }
}

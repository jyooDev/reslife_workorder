using System.ComponentModel.DataAnnotations;

namespace ReslifeWorkorderManagement.Models
{
    public class UserEditVM
    {

        [Required(ErrorMessage = "First name is required.")]
        public required string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public required string? LastName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [EmailAddress]
        public required string? Email { get; set; } 

        [DataType(DataType.Password)]
        public  string? OldPassword { get; set; }

        [DataType(DataType.Password)]
        public  string? NewPassword { get; set; }

        public required string Role { get; set; }
    }
}

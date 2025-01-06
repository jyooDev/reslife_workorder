using System.ComponentModel.DataAnnotations;

namespace ReslifeWorkorderManagement.Models
{
    public class HomeVM
    {

        public WorkOrder workOrder { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ReslifeWorkorderManagement.Models
{
    public class StaffVM
    {
        public required ApplicationUser user { get; set; }

        public required string role { get; set; }
        
    }
}

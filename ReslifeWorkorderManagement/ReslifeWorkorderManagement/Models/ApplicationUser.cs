using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace ReslifeWorkorderManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(30)]
        [Required]
        public string? FirstName { get; set; }

        [StringLength(30)]
        [Required]
        public string? LastName { get; set; }

        public ICollection<WorkOrder> AssignedWorkOrder { get; } = new List<WorkOrder>();
    }
}


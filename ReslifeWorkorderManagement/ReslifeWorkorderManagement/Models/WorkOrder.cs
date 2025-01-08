using System.ComponentModel.DataAnnotations;

namespace ReslifeWorkorderManagement.Models
{
    public class WorkOrder
    {
        public WorkOrder()
        {
            Progress = Progress.Requested;
        }

        public int Id { get; set; }
        public required string RequesterFirstName { get; set; }
        public required string RequesterLastName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public required string RequesterEmail { get; set; }
        public required string Request { get; set; }
        public required Building Building { get; set; }
        public required Area Area { get; set; }
        public int? Floor { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The Room Number must be 0 or greater.")]
        public int? RoomNumber { get; set; }
        public Progress Progress { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? CompletedAt{ get; set; }
        //foreignkey
        public string? WorkOrderAssigneeId { get; set; } //optional forein key property

        //Navigation properties
        public ApplicationUser? WorkOrderAssignee { get; set; } //optional reference navigation
    }

    public enum Area
    {
        Kitchen,
        Room,
        FemaleBathroom,
        MaleBathroom,
        GenderNeutralBathroom,
        Other
    }   
    public enum Building
    {
        Ross,
        Hawkes,
        Curran,
        Mcneil,
        Ostrander,
        Crownhart,
        YU,
        Swenson
    }

    public enum Progress
    {
        Requested,
        InProgress,
        Completed
    }
}

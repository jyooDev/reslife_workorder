using System.ComponentModel.DataAnnotations;

namespace ReslifeWorkorderManagement.Models
{
    public class History
    {
        public int Id { get; set; }
        public required string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //foreign key
        public required string ActionUserId { get; set; }

        //navigation
        public  ApplicationUser ActionUser { get; set; }
    }
}

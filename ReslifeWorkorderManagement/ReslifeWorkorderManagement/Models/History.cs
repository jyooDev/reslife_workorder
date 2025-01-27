using System.ComponentModel.DataAnnotations;

namespace ReslifeWorkorderManagement.Models
{
    public class History
    {
        public int Id { get; set; }
        public required string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public required HistoryType Type { get; set; }
        //foreign key
        public string ActionUserId { get; set; }

        public int WorkorderId { get; set; }

        //navigation
        public WorkOrder WorkOrder { get; set; }
        public  ApplicationUser ActionUser { get; set; }

    }

    public enum HistoryType
    {
        CREATE,
        EDIT,
        DELETE,
        ASSIGN,
        UPDATEPROGRESS
    }
}

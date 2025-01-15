namespace ReslifeWorkorderManagement.Models
{
    public class DashboardVM
    {

        public required int requestedWorkOrders{ get; set; }

        public required int inProgressWorkOrders { get; set; }

        public required int completeWorkOrders { get; set; }

        public required IEnumerable<WorkOrder> workOrders { get; set; }
    }
}

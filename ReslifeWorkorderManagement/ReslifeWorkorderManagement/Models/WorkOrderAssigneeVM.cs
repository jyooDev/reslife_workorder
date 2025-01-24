using Microsoft.AspNetCore.Mvc.Rendering;
namespace ReslifeWorkorderManagement.Models
{
    public class WorkOrderAssigneeVM
    {
        public IEnumerable<WorkOrder> WorkOrders { get; set; }
        public List<SelectListItem>? Assignees { get; set; }
    }
}

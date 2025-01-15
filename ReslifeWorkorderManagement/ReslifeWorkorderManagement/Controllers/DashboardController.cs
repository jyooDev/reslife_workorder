using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReslifeWorkorderManagement.Data;
using ReslifeWorkorderManagement.Models;

namespace ReslifeWorkorderManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WorkOrder.Include(w => w.WorkOrderAssignee);
            var workOrders = await applicationDbContext.ToListAsync();
                        workOrders.Sort((x, y) => DateTime.Compare(x.CreatedAt, y.CreatedAt));
            int requested = _context.WorkOrder.Where(w => w.Progress == Models.Progress.Requested).Count();
            int inProgress= _context.WorkOrder.Where(w => w.Progress == Models.Progress.InProgress).Count();
            int complete = _context.WorkOrder.Where(w => w.Progress == Models.Progress.Completed).Count();
            workOrders.Reverse();
            var recentFiveWorkOrders = workOrders.Take(5);

            DashboardVM model = new DashboardVM()
            {
                requestedWorkOrders = requested,
                inProgressWorkOrders = inProgress,
                completeWorkOrders = complete,
                workOrders = recentFiveWorkOrders
            };
            return View(model);
        }
    }
}

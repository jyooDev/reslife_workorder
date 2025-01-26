using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReslifeWorkorderManagement.Data;
using ReslifeWorkorderManagement.Models;

namespace ReslifeWorkorderManagement.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var workOrders = await _context.WorkOrder.Include(w => w.WorkOrderAssignee).ToListAsync();
            workOrders.Sort((x, y) => DateTime.Compare(x.CreatedAt, y.CreatedAt));
            workOrders.Reverse();
            var recentFiveWorkOrders = workOrders.Take(5);

            var history = await _context.History.Include(h => h.ActionUser).Where(h => h.Type != HistoryType.CREATE).ToListAsync();
            history.Sort((x, y) => DateTime.Compare(x.CreatedAt, y.CreatedAt));
            history.Reverse();
            var recentFiveUpdates = history.Take(5);
            int requested = _context.WorkOrder.Where(w => w.Progress == Models.Progress.Requested).Count();
            int inProgress= _context.WorkOrder.Where(w => w.Progress == Models.Progress.InProgress).Count();
            int complete = _context.WorkOrder.Where(w => w.Progress == Models.Progress.Completed).Count();

            DashboardVM model = new DashboardVM()
            {
                requestedWorkOrders = requested,
                inProgressWorkOrders = inProgress,
                completeWorkOrders = complete,
                workOrders = recentFiveWorkOrders,
                recentUpdates = recentFiveUpdates,
            };
            return View(model);
        }
    }
}

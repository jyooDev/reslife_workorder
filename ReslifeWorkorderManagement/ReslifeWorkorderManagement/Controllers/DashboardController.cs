using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReslifeWorkorderManagement.Data;

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
            workOrders.Reverse();
            var recentFiveWorkOrders = workOrders.Take(5);
            return View(recentFiveWorkOrders);
        }
    }
}

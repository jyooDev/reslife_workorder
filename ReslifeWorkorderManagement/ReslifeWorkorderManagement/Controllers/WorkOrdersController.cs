using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReslifeWorkorderManagement.Data;
using ReslifeWorkorderManagement.Models;

namespace ReslifeWorkorderManagement.Controllers
{

    public class WorkOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkOrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WorkOrders
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var workOrders = await _context.WorkOrder.Include(w => w.WorkOrderAssignee).ToListAsync();
            var users = await _context.Users.ToListAsync();
            var userroles = await _context.UserRoles.ToListAsync();
            var maintenanceRole = await _context.Roles.Where(r => r.Name == "MaintenanceStaff").FirstOrDefaultAsync();
            var maintenanceUsers = userroles.Where(ur => ur.RoleId == maintenanceRole.Id)
                .Join(users, ur => ur.UserId, u => u.Id, (ur, u) => u).ToList();

            var assigneeList = maintenanceUsers.Select(u => new SelectListItem
            {
                Text = $"{u.FirstName} {u.LastName}",
                Value = u.Id
            }).ToList();

            WorkOrderAssigneeVM model = new WorkOrderAssigneeVM()
            {
                WorkOrders = workOrders,
                Assignees = assigneeList
            };
            return View(model);
        }

        // GET: WorkOrders/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrder
                .Include(w => w.WorkOrderAssignee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workOrder == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsPartial", workOrder);
        }

        // GET: WorkOrders/Create
        public IActionResult Create()
        {
            ViewData["WorkOrderAssigneeId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: WorkOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> WorkorderSubmitted([Bind("RequesterFirstName,RequesterLastName,RequesterEmail,Request,Building,Area,Floor,RoomNumber")] WorkOrder workOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workOrder);
                var history = new History()
                {
                    Type = HistoryType.CREATE,
                    Message = "New workorder #" + workOrder.Id + " is submitted." ,
                    WorkOrder = workOrder
                };
                _context.Update(history);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Workorder is submitted successfully." });
            }
            return Json(new { success = false, message = "Error: Please enter all required fields." });
        }

        // GET: WorkOrders/Edit/5
        [HttpGet]
        [Authorize(Roles = "Administrator,StudentSupervisor,MaintenanceStaff")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrder.FindAsync(id);
            if (workOrder == null)
            {
                return NotFound();
            }
            WorkOrderVM model = new WorkOrderVM()
            {
                Note = workOrder.Note
            };

            ViewData["WorkOrderId"] = id;
            return PartialView("_EditWorkOrderPartial", model);
        }

        // POST: WorkOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Administrator,StudentSupervisor,MaintenanceStaff")]
        public async Task<IActionResult> Edit(int id, WorkOrderVM model)
        {
            var workOrder = await _context.WorkOrder.FindAsync(id);
            workOrder.Note = model.Note;

            var user = await _userManager.GetUserAsync(User);
            var history = new History()
            {
                Type = HistoryType.EDIT,
                Message = "has edited note of workordder #" + id + ".",
                ActionUserId = user.Id,
                WorkOrder = workOrder,
            };
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workOrder);
                    _context.Update(history);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkOrderExists(workOrder.Id))
                    {
                        return Json(new { success = false, message = "Workorder does not exist." });
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(new { success = true, message = "Note is edited successfully." });
            }
            return View(model);
        }

        // GET: WorkOrders/Delete/5
        [HttpGet]
        [Authorize]
        [Authorize(Roles = "Administrator,StudentSupervisor,MaintenanceStaff")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrder
                .Include(w => w.WorkOrderAssignee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workOrder == null)
            {
                return NotFound();
            }

            ViewData["WorkOrderId"] = id;
            return PartialView("_DeleteWorkOrderPartial", workOrder);
        }

        // POST: WorkOrders/Delete/5
        [HttpPost]
        [Authorize]
        [Authorize(Roles = "Administrator,StudentSupervisor,MaintenanceStaff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workOrder = await _context.WorkOrder.FindAsync(id);
            if (workOrder != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var history = new History()
                {
                    Type = HistoryType.DELETE,
                    Message = "has deleted workorder #" + workOrder.Id + ".",
                    ActionUserId = user.Id,
                };
                _context.Update(history);
                _context.WorkOrder.Remove(workOrder);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Workorder is deleted successfully." });
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles ="Administrator,StudentSupervisor,MaintenanceStaff")]
        public async Task<IActionResult> updateProgress(int id, string newProgress)
        {
            var workOrder = await _context.WorkOrder.FindAsync(id);

            if (workOrder == null)
            {
                return Json(new { success = false, message = "WorkOrder not found" });
            }
            try
            {
                if (Enum.TryParse(typeof(Progress), newProgress, out var statusValue))
                {
                    workOrder.Progress = (Progress)statusValue;
                }
                var user = await _userManager.GetUserAsync(User);
                var history = new History()
                {
                    Type = HistoryType.UPDATEPROGRESS,
                    Message = "has updated the progress of workorder #" + workOrder.Id + " to ",
                    ActionUserId = user.Id,
                    WorkOrder = workOrder
                };
                switch (workOrder.Progress)
                {
                    case Progress.Requested:
                        history.Message += "REQUESTED";
                        break;
                    case Progress.InProgress:
                        history.Message += "IN PROGRESS";
                        break;
                    case Progress.Completed:
                        history.Message += "COMPLETED";
                        break;
                }
                _context.Update(history);
                _context.Update(workOrder);
                await _context.SaveChangesAsync();
                return Json(new { success = true, workOrderId = id, progress = newProgress });

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Something went wrong while updaitng progress. Try again." });
            }
        }

        [HttpPost]
        [Route("WorkOrders/UpdateAssignee/{id}")]
        [Authorize]
        [Authorize(Roles = "Administrator,StudentSupervisor,MaintenanceStaff")]
        public async Task<IActionResult> UpdateAssignee(int id, [FromForm] string newAssigneeId)
        {
            var workOrder = await _context.WorkOrder.FindAsync(id);

            if (workOrder == null)
            {
                return Json(new { success = false, message = "WorkOrder not found" });
            }
            var assignee = await _context.Users.FindAsync(newAssigneeId);
            workOrder.WorkOrderAssignee = assignee;
            _context.Update(workOrder);
            var user = await _userManager.GetUserAsync(User);
            var history = new History()
            {
                Type = HistoryType.ASSIGN,
                Message = "has assigned workorder #" + workOrder.Id + " to " + assignee.FirstName + assignee.LastName,
                ActionUserId = user.Id
            };
            _context.Update(history);
            await _context.SaveChangesAsync();
            return Json(new { success = true, workOrderId = id, message = "New Assignee is selected." });
        }

        private bool WorkOrderExists(int id)
        {
            return _context.WorkOrder.Any(e => e.Id == id);
        }
    }
}

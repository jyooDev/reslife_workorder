using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReslifeWorkorderManagement.Data;
using ReslifeWorkorderManagement.Models;

namespace ReslifeWorkorderManagement.Controllers
{
    [Authorize]
    public class WorkOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkOrders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WorkOrder.Include(w => w.WorkOrderAssignee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WorkOrders/Details/5
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
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Workorder is submitted successfully." });
            }
            return Json(new { success = false, message = "Error: Please enter all required fields." });
        }

        // GET: WorkOrders/Edit/5
        [HttpGet]
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
        public async Task<IActionResult> Edit(int id, WorkOrderVM model)
        {
            var workOrder = await _context.WorkOrder.FindAsync(id);
            workOrder.Note = model.Note;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workOrder);
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workOrder = await _context.WorkOrder.FindAsync(id);
            if (workOrder != null)
            {
                _context.WorkOrder.Remove(workOrder);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Workorder is deleted successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> updateProgress(int id, string newProgress)
        {
            var workOrder = await _context.WorkOrder.FindAsync(id);

            if(workOrder == null)
            {
                return Json(new { success = false, message = "WorkOrder not found" });
            }
            try
            {
                if (Enum.TryParse(typeof(Progress), newProgress, out var statusValue))
                {
                    workOrder.Progress = (Progress)statusValue;
                }
                _context.Update(workOrder);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Progress updated successfully." });

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Something went wrong while updaitng progress. Try again." });
            }
        }

        private bool WorkOrderExists(int id)
        {
            return _context.WorkOrder.Any(e => e.Id == id);
        }
    }
}

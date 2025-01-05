using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            return View(workOrder);
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
            return Json(new { success = false, message = "Form is invalid" });
        }

        // GET: WorkOrders/Edit/5
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
            ViewData["WorkOrderAssigneeId"] = new SelectList(_context.Users, "Id", "Id", workOrder.WorkOrderAssigneeId);
            return View(workOrder);
        }

        // POST: WorkOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RequesterFirstName,RequesterLastName,RequesterEmail,Request,Building,Area,Floor,RoomNumber,Progress,Note,WorkOrderAssigneeId")] WorkOrder workOrder)
        {
            if (id != workOrder.Id)
            {
                return NotFound();
            }

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
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkOrderAssigneeId"] = new SelectList(_context.Users, "Id", "Id", workOrder.WorkOrderAssigneeId);
            return View(workOrder);
        }

        // GET: WorkOrders/Delete/5
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

            return View(workOrder);
        }

        // POST: WorkOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workOrder = await _context.WorkOrder.FindAsync(id);
            if (workOrder != null)
            {
                _context.WorkOrder.Remove(workOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkOrderExists(int id)
        {
            return _context.WorkOrder.Any(e => e.Id == id);
        }
    }
}

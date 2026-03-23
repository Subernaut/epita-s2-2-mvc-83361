using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Domain;
using Library.MVC.Data;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public class FollowUpsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FollowUpsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FollowUps
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FollowUps.Include(f => f.Inspection);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FollowUps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                Log.Warning("FollowUp details requested with null id by user {UserName}", User.Identity?.Name);
                return NotFound();
            }

            var followUp = await _context.FollowUps
                .Include(f => f.Inspection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (followUp == null)
            {
                Log.Warning("FollowUp details requested for non-existent FollowUpId {FollowUpId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            Log.Information("FollowUp details viewed. FollowUpId: {FollowUpId}, InspectionId: {InspectionId}", followUp.Id, followUp.InspectionId);
            return View(followUp);
        }

        // GET: FollowUps/Create
        public IActionResult Create()
        {
            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Notes");
            return View();
        }

        // POST: FollowUps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InspectionId,DueDate,Status,ClosedDate")] FollowUp followUp)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(followUp);
                    await _context.SaveChangesAsync();

                    Log.Information("FollowUp created. FollowUpId: {FollowUpId}, InspectionId: {InspectionId}", followUp.Id, followUp.InspectionId);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error creating FollowUp for InspectionId {InspectionId}", followUp.InspectionId);
                    throw;
                }
            }

            Log.Warning("Invalid FollowUp creation attempt by user {UserName}", User.Identity?.Name);
            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Notes", followUp.InspectionId);
            return View(followUp);
        }

        // GET: FollowUps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                Log.Warning("FollowUp edit requested with null id by user {UserName}", User.Identity?.Name);
                return NotFound();
            }

            var followUp = await _context.FollowUps.FindAsync(id);
            if (followUp == null)
            {
                Log.Warning("FollowUp edit requested for non-existent FollowUpId {FollowUpId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Notes", followUp.InspectionId);
            return View(followUp);
        }

        // POST: FollowUps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InspectionId,DueDate,Status,ClosedDate")] FollowUp followUp)
        {
            if (id != followUp.Id)
            {
                Log.Warning("FollowUp edit attempted with mismatched id {AttemptedId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(followUp);
                    await _context.SaveChangesAsync();

                    Log.Information("FollowUp updated. FollowUpId: {FollowUpId}, InspectionId: {InspectionId}", followUp.Id, followUp.InspectionId);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!FollowUpExists(followUp.Id))
                    {
                        Log.Warning("FollowUp edit failed due to non-existent FollowUpId {FollowUpId}", followUp.Id);
                        return NotFound();
                    }
                    Log.Error(ex, "Concurrency error editing FollowUpId {FollowUpId}", followUp.Id);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unexpected error editing FollowUpId {FollowUpId}", followUp.Id);
                    throw;
                }
            }

            Log.Warning("Invalid FollowUp edit attempt by user {UserName}", User.Identity?.Name);
            ViewData["InspectionId"] = new SelectList(_context.Inspections, "Id", "Notes", followUp.InspectionId);
            return View(followUp);
        }

        // GET: FollowUps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                Log.Warning("FollowUp delete requested with null id by user {UserName}", User.Identity?.Name);
                return NotFound();
            }

            var followUp = await _context.FollowUps
                .Include(f => f.Inspection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (followUp == null)
            {
                Log.Warning("FollowUp delete requested for non-existent FollowUpId {FollowUpId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            Log.Information("FollowUp delete page viewed. FollowUpId: {FollowUpId}, InspectionId: {InspectionId}", followUp.Id, followUp.InspectionId);
            return View(followUp);
        }

        // POST: FollowUps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var followUp = await _context.FollowUps.FindAsync(id);
            if (followUp != null)
            {
                try
                {
                    _context.FollowUps.Remove(followUp);
                    await _context.SaveChangesAsync();

                    Log.Information("FollowUp deleted. FollowUpId: {FollowUpId}, InspectionId: {InspectionId}", followUp.Id, followUp.InspectionId);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error deleting FollowUpId {FollowUpId}", followUp.Id);
                    throw;
                }
            }
            else
            {
                Log.Warning("Attempted to delete non-existent FollowUpId {FollowUpId} by user {UserName}", id, User.Identity?.Name);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FollowUpExists(int id)
        {
            return _context.FollowUps.Any(e => e.Id == id);
        }
    }
}
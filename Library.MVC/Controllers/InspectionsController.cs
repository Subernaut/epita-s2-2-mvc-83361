using System;
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
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InspectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inspections
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Inspections.Include(i => i.Premises);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inspections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                Log.Warning("Inspection details requested with null id by user {UserName}", User.Identity?.Name);
                return NotFound();
            }

            var inspection = await _context.Inspections
                .Include(i => i.Premises)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null)
            {
                Log.Warning("Inspection details requested for non-existent InspectionId {InspectionId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            Log.Information("Inspection details viewed. InspectionId: {InspectionId}, PremisesId: {PremisesId}", inspection.Id, inspection.PremisesId);
            return View(inspection);
        }

        // GET: Inspections/Create
        public IActionResult Create()
        {
            ViewData["PremisesId"] = new SelectList(_context.Premises, "Id", "Address");
            return View();
        }

        // POST: Inspections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PremisesId,InspectionDate,Score,Outcome,Notes")] Inspection inspection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(inspection);
                    await _context.SaveChangesAsync();

                    Log.Information("Inspection created. InspectionId: {InspectionId}, PremisesId: {PremisesId} by user {UserName}",
                        inspection.Id, inspection.PremisesId, User.Identity?.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error creating Inspection for PremisesId {PremisesId} by user {UserName}", inspection.PremisesId, User.Identity?.Name);
                    throw;
                }
            }

            Log.Warning("Invalid Inspection creation attempt by user {UserName}", User.Identity?.Name);
            ViewData["PremisesId"] = new SelectList(_context.Premises, "Id", "Address", inspection.PremisesId);
            return View(inspection);
        }

        // GET: Inspections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                Log.Warning("Inspection edit requested with null id by user {UserName}", User.Identity?.Name);
                return NotFound();
            }

            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null)
            {
                Log.Warning("Inspection edit requested for non-existent InspectionId {InspectionId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }
            ViewData["PremisesId"] = new SelectList(_context.Premises, "Id", "Address", inspection.PremisesId);
            return View(inspection);
        }

        // POST: Inspections/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PremisesId,InspectionDate,Score,Outcome,Notes")] Inspection inspection)
        {
            if (id != inspection.Id)
            {
                Log.Warning("Inspection edit attempted with mismatched id {AttemptedId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspection);
                    await _context.SaveChangesAsync();

                    Log.Information("Inspection updated. InspectionId: {InspectionId}, PremisesId: {PremisesId} by user {UserName}",
                        inspection.Id, inspection.PremisesId, User.Identity?.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!InspectionExists(inspection.Id))
                    {
                        Log.Warning("Inspection edit failed due to non-existent InspectionId {InspectionId}", inspection.Id);
                        return NotFound();
                    }
                    Log.Error(ex, "Concurrency error editing InspectionId {InspectionId} by user {UserName}", inspection.Id, User.Identity?.Name);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unexpected error editing InspectionId {InspectionId} by user {UserName}", inspection.Id, User.Identity?.Name);
                    throw;
                }
            }

            Log.Warning("Invalid Inspection edit attempt by user {UserName}", User.Identity?.Name);
            ViewData["PremisesId"] = new SelectList(_context.Premises, "Id", "Address", inspection.PremisesId);
            return View(inspection);
        }

        // GET: Inspections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                Log.Warning("Inspection delete requested with null id by user {UserName}", User.Identity?.Name);
                return NotFound();
            }

            var inspection = await _context.Inspections
                .Include(i => i.Premises)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null)
            {
                Log.Warning("Inspection delete requested for non-existent InspectionId {InspectionId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            Log.Information("Inspection delete page viewed. InspectionId: {InspectionId}, PremisesId: {PremisesId} by user {UserName}", inspection.Id, inspection.PremisesId, User.Identity?.Name);
            return View(inspection);
        }

        // POST: Inspections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection != null)
            {
                try
                {
                    _context.Inspections.Remove(inspection);
                    await _context.SaveChangesAsync();

                    Log.Information("Inspection deleted. InspectionId: {InspectionId}, PremisesId: {PremisesId} by user {UserName}",
                        inspection.Id, inspection.PremisesId, User.Identity?.Name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error deleting InspectionId {InspectionId} by user {UserName}", inspection.Id, User.Identity?.Name);
                    throw;
                }
            }
            else
            {
                Log.Warning("Attempted to delete non-existent InspectionId {InspectionId} by user {UserName}", id, User.Identity?.Name);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InspectionExists(int id)
        {
            return _context.Inspections.Any(e => e.Id == id);
        }
    }
}
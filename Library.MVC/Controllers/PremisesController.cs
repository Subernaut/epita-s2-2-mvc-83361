using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.Domain;
using Library.MVC.Data;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace Library.MVC.Controllers
{
    [Authorize(Roles = "Admin,Inspector,Viewer")]
    public class PremisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PremisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Premises
        public async Task<IActionResult> Index()
        {
            Log.Information("Premises index viewed by user {UserName}", User.Identity?.Name);
            return View(await _context.Premises.ToListAsync());
        }

        // GET: Premises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                Log.Warning("Premises details requested with null id by user {UserName}", User.Identity?.Name);
                return NotFound();
            }

            var premise = await _context.Premises.FirstOrDefaultAsync(m => m.Id == id);
            if (premise == null)
            {
                Log.Warning("Premises details requested for non-existent PremiseId {PremiseId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            Log.Information("Premises details viewed. PremiseId: {PremiseId}, Name: {Name} by user {UserName}", premise.Id, premise.Name, User.Identity?.Name);
            return View(premise);
        }

        // GET: Premises/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Premises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Town,RiskRating")] Premise premise)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(premise);
                    await _context.SaveChangesAsync();
                    Log.Information("Premise created. PremiseId: {PremiseId}, Name: {Name} by user {UserName}", premise.Id, premise.Name, User.Identity?.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error creating Premise with Name {Name} by user {UserName}", premise.Name, User.Identity?.Name);
                    throw;
                }
            }

            Log.Warning("Invalid Premise creation attempt by user {UserName}", User.Identity?.Name);
            return View(premise);
        }

        // GET: Premises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                Log.Warning("Premises edit requested with null id by user {UserName}", User.Identity?.Name);
                return NotFound();
            }

            var premise = await _context.Premises.FindAsync(id);
            if (premise == null)
            {
                Log.Warning("Premises edit requested for non-existent PremiseId {PremiseId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            return View(premise);
        }

        // POST: Premises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Town,RiskRating")] Premise premise)
        {
            if (id != premise.Id)
            {
                Log.Warning("Premises edit attempted with mismatched id {AttemptedId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(premise);
                    await _context.SaveChangesAsync();
                    Log.Information("Premise updated. PremiseId: {PremiseId}, Name: {Name} by user {UserName}", premise.Id, premise.Name, User.Identity?.Name);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!PremiseExists(premise.Id))
                    {
                        Log.Warning("Premise edit failed due to non-existent PremiseId {PremiseId}", premise.Id);
                        return NotFound();
                    }
                    Log.Error(ex, "Concurrency error editing PremiseId {PremiseId} by user {UserName}", premise.Id, User.Identity?.Name);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unexpected error editing PremiseId {PremiseId} by user {UserName}", premise.Id, User.Identity?.Name);
                    throw;
                }
            }

            Log.Warning("Invalid Premise edit attempt by user {UserName}", User.Identity?.Name);
            return View(premise);
        }

        // GET: Premises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                Log.Warning("Premises delete requested with null id by user {UserName}", User.Identity?.Name);
                return NotFound();
            }

            var premise = await _context.Premises.FirstOrDefaultAsync(m => m.Id == id);
            if (premise == null)
            {
                Log.Warning("Premises delete requested for non-existent PremiseId {PremiseId} by user {UserName}", id, User.Identity?.Name);
                return NotFound();
            }

            Log.Information("Premise delete page viewed. PremiseId: {PremiseId}, Name: {Name} by user {UserName}", premise.Id, premise.Name, User.Identity?.Name);
            return View(premise);
        }

        // POST: Premises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var premise = await _context.Premises.FindAsync(id);
            if (premise != null)
            {
                try
                {
                    _context.Premises.Remove(premise);
                    await _context.SaveChangesAsync();
                    Log.Information("Premise deleted. PremiseId: {PremiseId}, Name: {Name} by user {UserName}", premise.Id, premise.Name, User.Identity?.Name);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error deleting PremiseId {PremiseId} by user {UserName}", premise.Id, User.Identity?.Name);
                    throw;
                }
            }
            else
            {
                Log.Warning("Attempted to delete non-existent PremiseId {PremiseId} by user {UserName}", id, User.Identity?.Name);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PremiseExists(int id)
        {
            return _context.Premises.Any(e => e.Id == id);
        }
    }
}
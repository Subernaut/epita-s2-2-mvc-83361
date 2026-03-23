using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library.MVC.Data;
using Library.Domain;
using Library.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Library.MVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? townFilter, string? riskRatingFilter)
        {
            var inspectionsQuery = _context.Inspections
                .Include(i => i.Premises)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(townFilter))
                inspectionsQuery = inspectionsQuery.Where(i => i.Premises.Town == townFilter);

            RiskRating? riskEnum = null;
            if (!string.IsNullOrEmpty(riskRatingFilter) && Enum.TryParse<RiskRating>(riskRatingFilter, out var parsedRisk))
                riskEnum = parsedRisk;

            if (riskEnum.HasValue)
                inspectionsQuery = inspectionsQuery.Where(i => i.Premises.RiskRating == riskEnum.Value);

            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // Count inspections this month
            var inspectionsThisMonthCount = await inspectionsQuery
                .Where(i => i.InspectionDate >= startOfMonth)
                .CountAsync();

            // Count failed inspections this month (Outcome is enum)
            var failedInspectionsThisMonthCount = await inspectionsQuery
                .Where(i => i.InspectionDate >= startOfMonth && i.Outcome == Outcome.Fail)
                .CountAsync();

            // Count open overdue follow-ups (Status is enum)
            var followUpsQuery = _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i.Premises)
                .Where(f => f.Status == FollowUpStatus.Open && f.DueDate < today)
                .AsQueryable();

            if (!string.IsNullOrEmpty(townFilter))
                followUpsQuery = followUpsQuery.Where(f => f.Inspection.Premises.Town == townFilter);

            if (riskEnum.HasValue)
                followUpsQuery = followUpsQuery.Where(f => f.Inspection.Premises.RiskRating == riskEnum.Value);

            var openFollowUpsOverdueCount = await followUpsQuery.CountAsync();

            // Dropdown lists
            var towns = await _context.Premises
                .Select(p => p.Town)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            var riskRatings = Enum.GetValues(typeof(RiskRating)).Cast<RiskRating>().ToList();

            ViewBag.TownList = towns.Select(t => new SelectListItem { Text = t, Value = t }).ToList();
            ViewBag.RiskRatingList = riskRatings.Select(r => new SelectListItem { Text = r.ToString(), Value = r.ToString() }).ToList();
            ViewBag.SelectedTown = townFilter;
            ViewBag.SelectedRiskRating = riskRatingFilter;

            // Build ViewModel
            var model = new DashboardViewModel
            {
                InspectionsThisMonthCount = inspectionsThisMonthCount,
                FailedInspectionsThisMonthCount = failedInspectionsThisMonthCount,
                OpenFollowUpsOverdueCount = openFollowUpsOverdueCount,
                SelectedTown = townFilter,
                SelectedRiskRating = riskRatingFilter,
                Towns = towns,
                RiskRatings = riskRatings.Select(r => r.ToString()).ToList()
            };

            return View(model);
        }
    }
}
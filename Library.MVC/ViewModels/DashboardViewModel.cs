using System.Collections.Generic;

namespace Library.MVC.ViewModels
{
    public class DashboardViewModel
    {
        public int InspectionsThisMonthCount { get; set; }
        public int FailedInspectionsThisMonthCount { get; set; }
        public int OpenFollowUpsOverdueCount { get; set; }

        public string? SelectedTown { get; set; }
        public string? SelectedRiskRating { get; set; }

        public List<string> Towns { get; set; } = new();
        public List<string> RiskRatings { get; set; } = new();
    }
}
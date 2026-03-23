using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    public class Inspection
    {
        public int Id { get; set; }

        // FK
        public int PremisesId { get; set; }
        public Premise Premises { get; set; } = null!;

        public DateTime InspectionDate { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }

        [Required]
        public Outcome Outcome { get; set; }

        public string Notes { get; set; } = string.Empty;

        public ICollection<FollowUp> FollowUps { get; set; } = new List<FollowUp>();
    }
}

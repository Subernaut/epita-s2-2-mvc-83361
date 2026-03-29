using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    public class FollowUp
    {
        public int Id { get; set; }

        // FK
        public int InspectionId { get; set; }
        public Inspection? Inspection { get; set; } = null!;

        public DateTime DueDate { get; set; }

        public FollowUpStatus Status { get; set; }

        public DateTime? ClosedDate { get; set; }
    }
}

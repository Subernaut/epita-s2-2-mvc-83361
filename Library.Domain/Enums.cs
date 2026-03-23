using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain
{
    public enum RiskRating
    {
        Low,
        Medium,
        High
    }

    public enum Outcome
    {
        Pass,
        Fail
    }

    public enum FollowUpStatus
    {
        Open,
        Closed
    }
}

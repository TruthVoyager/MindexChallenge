using System;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        public int CompensationId { get; set; } // Primary key
        public Employee Employee { get; set; }

        public decimal Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}

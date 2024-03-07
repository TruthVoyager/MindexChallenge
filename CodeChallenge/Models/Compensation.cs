using System;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        public string EmployeeId { get; set; }
        public decimal Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}

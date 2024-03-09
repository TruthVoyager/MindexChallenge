using System;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        [Required]
        public string EmployeeId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        public decimal Salary { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }
    }
}

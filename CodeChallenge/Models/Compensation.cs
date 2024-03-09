using System;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        [Required]
        public string employee { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        public decimal salary { get; set; }

        [Required]
        public DateTime effectiveDate { get; set; }
    }
}

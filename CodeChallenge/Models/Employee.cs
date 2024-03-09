using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models
{
    public class Employee
    {
        public String EmployeeId { get; set; }
        [StringLength(100)]
        public String FirstName { get; set; }
        [StringLength(100)]
        public String LastName { get; set; }
        [StringLength(100)]
        public String Position { get; set; }
        [StringLength(100)]
        public String Department { get; set; }
        public List<Employee> DirectReports { get; set; }
    }
}

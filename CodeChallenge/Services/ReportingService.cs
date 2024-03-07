using CodeChallenge.Models;
using CodeChallenge.Services;

namespace CodeChallenge.Reporting
{
    public class ReportingService
    {
        private readonly EmployeeService _employeeService;

        public ReportingService(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        private int CalculateNumberOfReportsRecursive(Employee employee)
        {
            int numberOfReports = 0;

            // Count the direct reports of the current employee
            numberOfReports += employee.DirectReports?.Count ?? 0;

            // Recursively count the reports of each direct report
            if (employee.DirectReports != null)
            {
                foreach (Employee Report in employee.DirectReports)
                {
                    Employee directReport = _employeeService.GetById(Report.EmployeeId);
                    if (directReport != null)
                    {
                        numberOfReports += CalculateNumberOfReportsRecursive(directReport);
                    }
                }
            }

            return numberOfReports;
        }
    }
}

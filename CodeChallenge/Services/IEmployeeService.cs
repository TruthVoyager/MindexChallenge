using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Task<Employee> GetByIdAsync(String id);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee> ReplaceAsync(Employee originalEmployee, Employee newEmployee);
        Task<ReportingStructure> GetReportingServiceByIdAsync(string id);
        Task<Compensation> CreateCompensationAsync(Compensation compensation);
        Task<Compensation> GetCompensationByEmployeeIdAsync(string employeeId);
    }
}

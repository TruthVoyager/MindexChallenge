using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public async Task<Employee> GetByIdAsync(string id) // Updated return type to Task<Employee>
        {
            return await _employeeContext.Employees
                .Include(e => e.DirectReports)
                .SingleOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task SaveAsync() // Updated to be asynchronous
        {
            try
            {
                await Task.Run(() => _employeeContext.SaveChangesAsync());
                _logger.LogInformation("Changes saved to the database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save changes to the database.");
                throw;
            }
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        // Adds a new Compensation record to the database.
        public Compensation AddCompensation(Compensation compensation)
        {
            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }

        // Retrieves a Compensation record by employee ID from the database.
        public async Task<Compensation> GetCompensationByEmployeeIdAsync(string employeeId)
        {
            return await _employeeContext.Compensations.FirstOrDefaultAsync(c => c.EmployeeId == employeeId);
        }
    }
}

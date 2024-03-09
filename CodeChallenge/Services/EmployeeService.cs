using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<Employee> Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                await _employeeRepository.SaveAsync();
            }

            return employee;
        }

        public async Task<Employee> GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return await _employeeRepository.GetByIdAsync(id);
            }

            return null;
        }

        // Retrieves the reporting structure for a given employee, calculating the total number of reports.
        public async Task<ReportingStructure> GetReportingServiceById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee != null)
                {
                    int numberOfReports = await CountDirectReportsAsync(employee);
                    return new ReportingStructure
                    {
                        employee = employee.EmployeeId,
                        numberOfReports = numberOfReports
                    };
                }
            }

            return null;
        }

        private async Task<int> CountDirectReportsAsync(Employee employee)
        {
            int count = 0;
            if (employee.DirectReports != null)
            {
                foreach (var report in employee.DirectReports)
                {
                    var directReport = await _employeeRepository.GetByIdAsync(report.EmployeeId);
                    if (directReport != null)
                    {
                        count++;
                        count += await CountDirectReportsAsync(directReport);
                    }
                }
            }
            return count;
        }

        public async Task<Employee> Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    await _employeeRepository.SaveAsync();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                await _employeeRepository.SaveAsync();
            }

            return newEmployee;
        }

        // Creates a new Compensation record and saves it to the database.
        public async Task<Compensation> CreateCompensation(Compensation compensation)
        {
            if (compensation == null)
            {
                throw new ArgumentNullException(nameof(compensation));
            }

            // Save compensation to database
            var savedCompensation = _employeeRepository.AddCompensation(compensation);
            await _employeeRepository.SaveAsync();

            return savedCompensation;
        }

        // Retrieves a Compensation record by employee ID.
        public async Task<Compensation> GetCompensationByEmployeeId(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                throw new ArgumentNullException(nameof(employeeId));
            }

            return await _employeeRepository.GetCompensationByEmployeeIdAsync(employeeId);
        }
    }
}

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

        public async Task<Employee> CreateAsync(Employee employee)
        {
            try{
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                await _employeeRepository.SaveAsync();
            }

            return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create employee.");
                throw;
            }
        }

        public async Task<Employee> GetByIdAsync(string id)
        {
            try{
            if(!String.IsNullOrEmpty(id))
            {
                return await _employeeRepository.GetByIdAsync(id);
            }

            return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve employee by id.");
                throw;
            }
        }

        // Retrieves the reporting structure for a given employee, calculating the total number of reports.
        public async Task<ReportingStructure> GetReportingServiceByIdAsync(string id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve reporting structure.");
                throw;
            }
        }

        private async Task<int> CountDirectReportsAsync(Employee employee)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to count direct reports.");
                throw;
            }
        }

        public async Task<Employee> ReplaceAsync(Employee originalEmployee, Employee newEmployee)
        {
            try
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
            catch (System.Exception)
            {
                _logger.LogError("Failed to replace employee.");
                throw;
            }
            
        }

        // Creates a new Compensation record and saves it to the database.
        public async Task<Compensation> CreateCompensationAsync(Compensation compensation)
        {
            try
            {
                if (compensation == null)
                {
                    throw new ArgumentNullException(nameof(compensation));
                }

                // Save compensation to database
                var savedCompensation = _employeeRepository.AddCompensation(compensation);
                await _employeeRepository.SaveAsync();

                _logger.LogInformation("Compensation created successfully.");

                return savedCompensation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create compensation.");
                throw;
            }
        }

        // Retrieves a Compensation record by employee ID.
        public async Task<Compensation> GetCompensationByEmployeeIdAsync(string employeeId)
        {
            try
            {
                if (string.IsNullOrEmpty(employeeId))
                {
                    throw new ArgumentNullException(nameof(employeeId));
                }

                return await _employeeRepository.GetCompensationByEmployeeIdAsync(employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve compensation.");
                throw;
            }
        }
    }
}

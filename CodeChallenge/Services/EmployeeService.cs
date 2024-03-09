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

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        // Retrieves the reporting structure for a given employee, calculating the total number of reports.
        public ReportingStructure GetReportingServiceById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var employee = _employeeRepository.GetById(id);
                if (employee != null)
                {
                    int numberOfReports = CountDirectReports(employee);
                    return new ReportingStructure
                    {
                        employee = employee.EmployeeId,
                        numberOfReports = numberOfReports
                    };
                }
            }

            return null;
        }

        private int CountDirectReports(Employee employee)
        {
            int count = 0;
            if (employee.DirectReports != null)
            {
                foreach (var report in employee.DirectReports)
                {
                    var directReport = _employeeRepository.GetById(report.EmployeeId);
                    if (directReport != null)
                    {
                        count++;
                        count += CountDirectReports(directReport);
                    }
                }
            }
            return count;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        // Creates a new Compensation record and saves it to the database.
        public Compensation CreateCompensation(Compensation compensation)
        {
            if (compensation == null)
            {
                throw new ArgumentNullException(nameof(compensation));
            }

            // Save compensation to database
            var savedCompensation = _employeeRepository.AddCompensation(compensation);
            _employeeRepository.SaveAsync().Wait();

            return savedCompensation;
        }

        // Retrieves a Compensation record by employee ID.
        public Compensation GetCompensationByEmployeeId(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                throw new ArgumentNullException(nameof(employeeId));
            }

            return _employeeRepository.GetCompensationByEmployeeId(employeeId);
        }
    }
}

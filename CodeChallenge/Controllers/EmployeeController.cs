﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAsync([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            await _employeeService.CreateAsync(employee);

            var result = CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);

            return result;
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public async Task<IActionResult> GetEmployeeByIdAsync(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ReplaceEmployeeAsync(String id, [FromBody]Employee newEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = await _employeeService.GetByIdAsync(id);
            if (existingEmployee == null)
                return NotFound();

            newEmployee = await _employeeService.ReplaceAsync(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        // GetReportingStructure
        // Purpose: Retrieves the reporting structure for a given employee.
        // Endpoint: GET /api/employee/getReportingStructure/{employeeId}
        // Input: Employee ID as a path variable.
        // Output: Returns a ReportingStructure object containing the employee ID and the total 
        //     number of reports under that employee, with HTTP status code 200 (OK) if the employee exists, 
        //     otherwise 404 (Not Found).
        // Implementation: This method fetches the reporting structure using the GetReportingServiceById 
        //     method of the EmployeeService. If the employee exists, it calculates the total number of 
        //     reports and returns the ReportingStructure; otherwise, it returns a 404 status code.
        [HttpGet("getReportingStructure/{employeeId}")]
        public async Task<IActionResult> GetReportingStructureAsync(string employeeId)
        {
            var reportingStructure = await _employeeService.GetReportingServiceByIdAsync(employeeId);
            if (reportingStructure == null)
            {
                return NotFound();
            }
            return Ok(reportingStructure);
        }
 
        // CreateCompensation
        // Purpose: Creates a new compensation record for an employee.
        // Endpoint: POST /api/employee/createCompensation
        // Input: A Compensation object containing the employee ID, salary, and effective date.
        // Output: Returns the created Compensation object with HTTP status code 201 (Created).
        // Implementation: This method attempts to create a compensation record using the CreateCompensation 
        //     method of the EmployeeService. If successful, it returns the created compensation object. 
        //     In case of failure, it logs the error and returns a 500 status code. 
        [HttpPost("createCompensation")]
        public async Task<IActionResult> CreateCompensationAsync([FromBody] Compensation compensation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdCompensation = await _employeeService.CreateCompensationAsync(compensation);
                var result = CreatedAtRoute("getCompensationById", new { employeeId = createdCompensation.employee }, createdCompensation);
                return result;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Failed to create compensation due to null argument");
                return BadRequest("Invalid input: " + ex.ParamName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create compensation");
                return StatusCode(500, "Internal server error");
            }
        }

        // GetCompensationById
        // Purpose: Retrieves a compensation record by employee ID.
        // Endpoint: GET /api/employee/getCompensationById/{employeeId}
        // Input: Employee ID as a path variable.
        // Output: Returns the corresponding Compensation object with HTTP status code 200 (OK) if found, 
        //     otherwise 404 (Not Found).
        // Implementation: This method fetches a compensation record using the GetCompensationByEmployeeId 
        //     method of the EmployeeService. If a record is found, it is returned; otherwise, a 404 status 
        //     code is returned.
        [HttpGet("getCompensationById/{employeeId}", Name = "getCompensationById")]
        public async Task<IActionResult> GetCompensationByIdAsync(string employeeId)
        {
            var compensation = await _employeeService.GetCompensationByEmployeeIdAsync(employeeId);
            if (compensation == null)
            {
                _logger.LogWarning($"Compensation record not found for employeeId: {employeeId}");
                return NotFound($"Compensation record not found for employeeId: {employeeId}");
            }
            return Ok(compensation);
        }
    }
}

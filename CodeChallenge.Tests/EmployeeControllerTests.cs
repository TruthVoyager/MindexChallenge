using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }
            if (_testServer != null)
            {
                _testServer.Dispose();
            }
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Tests the creation of a new employee compensation record. It sends a POST request with a Compensation object
        // and expects a response status code of Created (201). It then verifies that the returned Compensation object
        // matches the sent object.
        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var compensation = new Compensation()
            {
                employee = "16a596ae-edd3-4847-99fe-c4518e82c86f",
                salary = 50000,
                effectiveDate = DateTime.Now
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee/createCompensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation);
            Assert.AreEqual(compensation.employee, newCompensation.employee);
            Assert.AreEqual(compensation.salary, newCompensation.salary);
            Assert.AreEqual(compensation.effectiveDate, newCompensation.effectiveDate);
        }

        // Tests the retrieval of an employee by ID. It sends a GET request for an existing employee's compensation
        // and expects a response status code of OK (200). It then verifies that the returned Compensation object
        // matches the expected values for employeeId, salary, and effectiveDate.
        [TestMethod]
        public void GetCompensation_Returns_Ok()
        {
            // Arrange
            var compensation = new Compensation()
            {
                employee = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                salary = 50000,
                effectiveDate = DateTime.Now
            };
            // Create the compensation entry
            var requestContent = new JsonSerialization().ToJson(compensation);
            var postRequestTask = _httpClient.PostAsync("api/employee/createCompensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var postResponse = postRequestTask.Result;
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/getCompensationById/{compensation.employee}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var returnedCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(returnedCompensation);
            Assert.AreEqual(compensation.employee, returnedCompensation.employee);
            Assert.AreEqual(compensation.salary, returnedCompensation.salary);
            Assert.AreEqual(compensation.effectiveDate, returnedCompensation.effectiveDate);
        }

        // Tests the retrieval of the reporting structure for a given employee. It sends a GET request for the
        // reporting structure of an employee by ID and expects a response status code of OK (200). It then verifies
        // that the returned ReportingStructure object contains the correct employeeId and numberOfReports.
        [TestMethod]
        public void GetReportingStructure_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/getReportingStructure/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.IsNotNull(reportingStructure);
            Assert.AreEqual(employeeId, reportingStructure.employee);
            Assert.AreEqual(4, reportingStructure.numberOfReports);
        }
    }
}

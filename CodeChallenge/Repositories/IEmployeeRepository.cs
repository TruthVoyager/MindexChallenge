using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(String id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Compensation AddCompensation(Compensation compensation);
        Task<Compensation> GetCompensationByEmployeeIdAsync(string employeeId);
        Task SaveAsync();
    }
}
using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Data
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Compensation> Compensations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define primary key for Compensation entity
            modelBuilder.Entity<Compensation>().HasKey(c => c.EmployeeId);

            base.OnModelCreating(modelBuilder);
        }


    }
}

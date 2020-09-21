using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Pronali.Data.Repositories.Hr
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly ApplicationDbContext db;
        public EmployeeRepository(ApplicationDbContext _dbContext) : base(_dbContext)
        {
            db = _dbContext;
        }

        public Employee GetFirstOrDefaultWithRelatedData(Func<Employee, bool> predicate)
        {
            try
            {
                Employee employee = db.Employee
                    .Include(c => c.Department)
                    .Include(c => c.Designation)
                    .Include(c => c.Company)
                    .Include(c => c.Branch)
                    .Include(c => c.Machine)
                    .Include(c => c.Weekend)
                    .Include(c => c.Holidays).ThenInclude(c=>c.Holiday)
                    .Include(c => c.Section)
                    .Include(c => c.Shift)
                    .Include(c => c.Shift.ShiftDetailsList)
                    .Include(c => c.Line)
                    .Include(c => c.Floor)
                    .Include(c => c.JobLocation)
                    .Include(c => c.EmployeeGroup)
                    .Include(c => c.EmployeeLeaveList).ThenInclude(c => c.Leave)
                    .FirstOrDefault(predicate);

                return employee;
            }
            catch(Exception ex)
            {
                return new Employee();
            }

        }
        public List<Employee> GetAllWithRelatedData(Func<Employee, bool> predicate)
        {
            return db.Employee
                .Include(c => c.Department)
                .Include(c => c.Designation)
                .Include(c => c.Company)
                .Include(c => c.Branch)
                .Include(c => c.Shift).ThenInclude(c=>c.ShiftDetailsList)
                .Include(c => c.JobLocation)
                .Include(c => c.EmployeeGroup)
                .Include(c => c.EmployeeLeaveList).ThenInclude(c => c.Leave)
                .Where(predicate)
                .ToList();
        }

      
    }
}

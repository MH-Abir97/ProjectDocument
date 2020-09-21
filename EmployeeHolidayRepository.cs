using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;

namespace Pronali.Data.Repositories.Hr
{
    public class EmployeeHolidayRepository : BaseRepository<EmployeeHoliday>, IEmployeeHolidayRepository
    {
        private readonly ApplicationDbContext db;
        public EmployeeHolidayRepository(ApplicationDbContext _dbContext) : base(_dbContext)
        {
            db = _dbContext;
        }
        //public EmployeeHoliday GetFirstOrDefaultWithRelatedData(Func<Employee, bool> predicate)
        //{
        //    try
        //    {
        //        EmployeeHoliday employee = db.EmployeeHoliday
        //            .Include(c => c.Department)
        //            .Include(c => c.Designation)
        //            .Include(c => c.Company)
        //            .Include(c => c.Branch)
        //            .Include(c => c.Machine)
        //            .Include(c => c.Weekend)
        //            .Include(c => c.Holidays)
        //            .Include(c => c.Section)
        //            .Include(c => c.Shift)
        //            .Include(c => c.Shift.ShiftDetailsList)
        //            .Include(c => c.Line)
        //            .Include(c => c.Floor)
        //            .Include(c => c.JobLocation)
        //            .Include(c => c.EmployeeGroup)
        //            .Include(c => c.EmployeeLeaveList).ThenInclude(c => c.Leave)
        //            .FirstOrDefault(predicate);

        //        return employee;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new EmployeeHoliday();
        //    }

        //}
    }
}

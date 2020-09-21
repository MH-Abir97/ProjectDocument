using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;

namespace Pronali.Data.Repositories.Hr
{
    public class DailyAttendanceRepository : BaseRepository<DailyAttendance>, IDailyAttendanceRepository
    {
        private readonly ApplicationDbContext db;
        public DailyAttendanceRepository(ApplicationDbContext _context) : base(_context)
        {
            this.db = _context;
        }
        public List<DailyAttendance> GetAllWithRelatedData(Func<DailyAttendance, bool> predicate)
        {
            return db.DailyAttendance
                .Include(c => c.Employee)
                .Include(c => c.Employee.Department)
                .Include(c => c.Employee.Designation)
                .Include(c => c.Employee.Shift).ThenInclude(c=>c.ShiftDetailsList)
                .Include(c => c.Employee.JobLocation)
                .Where(predicate)
                .ToList();
        }
    }
}

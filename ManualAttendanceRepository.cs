using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
   public class ManualAttendanceRepository: BaseRepository<ManualAttendance>, IManualAttendanceRepository
    {
        private readonly ApplicationDbContext db;
        public ManualAttendanceRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
        public List<ManualAttendance> GetAllWithRelatedData()
        {
            try
            {
                List<ManualAttendance> attendances = db.ManualAttendance
                    .Include(d => d.Employee)
                    .Include(d => d.Approver)
                    .Where(x => x.IsDeleted == false)
                    .ToList();
                return attendances;
            }
            catch (Exception)
            {
                return new List<ManualAttendance>();
            }
        }

        public ManualAttendance GetFirstOrDefaultwithRelatedData(Func<ManualAttendance, bool> predicate)
        {
            return _entities
                .Include(c => c.Employee)
                .Include(c => c.Approver)
                .FirstOrDefault(predicate);
        }
    }
}

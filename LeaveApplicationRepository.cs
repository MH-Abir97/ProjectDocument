using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    public class LeaveApplicationRepository : BaseRepository<LeaveApplication>, ILeaveApplicationRepository
    {
        private readonly ApplicationDbContext db;
        public LeaveApplicationRepository(ApplicationDbContext _context) : base(_context)
        {
            this.db = _context;
        }

        public  IEnumerable<LeaveApplication> FindWithRelatedData(Func<LeaveApplication, bool> predicate)
        {
            return _entities
                .Include(c => c.Leave)
                .Include(c => c.Employee)
                .Include(c => c.Approver)
                .Where(predicate).OrderBy(o=>o.Status).ThenByDescending(t=>t.FromDate);
        }

        public LeaveApplication GetFirstOrDefaultwithRelatedData(Func<LeaveApplication, bool> predicate)
        {
            return _entities
                .Include(c => c.Leave)
                .Include(c => c.Employee)
                .Include(c => c.Approver)
                .FirstOrDefault(predicate);
        }


    }
}

using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    public class LatePermissionRepository : BaseRepository<LatePermission>, ILatePermissionRepository
    {
        private readonly ApplicationDbContext db;
        public LatePermissionRepository(ApplicationDbContext _context) :base(_context)
        {
            db = _context;
        }

        public List<LatePermission> FindWithRelatedData(Func<LatePermission, bool> predicate)
        {
            return _entities.Include(c => c.Employee).Where(predicate).ToList();
        }
        public LatePermission GetFirstOrDefaultwithRelatedData(Func<LatePermission, bool> predicate)
        {
            return _entities
                .Include(c => c.Employee)
                .Include(c => c.Approver)
                .FirstOrDefault(predicate);
        }
    }
}

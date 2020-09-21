using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    public class EarlyOutPermissionRepository : BaseRepository<EarlyOutPermission>, IEarlyOutPermissionRepository
    {
        private readonly ApplicationDbContext db;
        public EarlyOutPermissionRepository(ApplicationDbContext _context) :base(_context)
        {
            db = _context;
        }

        public List<EarlyOutPermission> FindWithRelatedData(Func<EarlyOutPermission, bool> predicate)
        {
            return _entities.Include(c => c.Employee).Where(predicate).ToList();
        }
        public EarlyOutPermission GetFirstOrDefaultwithRelatedData(Func<EarlyOutPermission, bool> predicate)
        {
            return _entities
                .Include(c => c.Employee)
                .Include(c => c.Approver)
                .FirstOrDefault(predicate);
        }
    }
}

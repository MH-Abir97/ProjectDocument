using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    public class BusinessApplicationRepository : BaseRepository<BusinessApplication>, IBusinessApplicationRepository
    {
        private readonly ApplicationDbContext db;
        public BusinessApplicationRepository(ApplicationDbContext _context) :base(_context)
        {
            db = _context;
        }

        public List<BusinessApplication> FindWithRelatedData(Func<BusinessApplication, bool> predicate)
        {
            return _entities.Include(c => c.Employee).Where(predicate).ToList();
        }
        public BusinessApplication GetFirstOrDefaultwithRelatedData(Func<BusinessApplication, bool> predicate)
        {
            return _entities
                .Include(c => c.Employee)
                .Include(c => c.Approver)
                .FirstOrDefault(predicate);
        }
    }
}

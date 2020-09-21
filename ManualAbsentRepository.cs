using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pronali.Data.Repositories.Hr
{
    public class ManualAbsentRepository: BaseRepository<ManualAbsent>, IManualAbsentRepository
    {
        private readonly ApplicationDbContext db;
        public ManualAbsentRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
        public List<ManualAbsent> GetAllWithRelatedData()
        {
            try
            {
                List<ManualAbsent> absents = db.ManualAbsent
                    .Include(d => d.Employee)
                    .Include(d => d.Approver)
                    .Where(x => x.IsDeleted == false)
                    .ToList();
                return absents;
            }
            catch (Exception)
            {
                return new List<ManualAbsent>();
            }
        }
        public ManualAbsent GetFirstOrDefaultwithRelatedData(Func<ManualAbsent, bool> predicate)
        {
            return _entities
                .Include(c => c.Employee)
                .Include(c => c.Approver)
                .FirstOrDefault(predicate);
        }
    }
}

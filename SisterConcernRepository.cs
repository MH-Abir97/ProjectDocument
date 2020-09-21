using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;

namespace Pronali.Data.Repositories.Core
{
    public class SisterConcernRepository : BaseRepository<SisterConcern>, ISisterConcernRepository
    {
        private readonly ApplicationDbContext db;
        public SisterConcernRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public IEnumerable<SisterConcern> GetAllWithRelatedData(Func<SisterConcern, bool> predicate)
        {
            try
            {
                List<SisterConcern> sisterConcerns = db.SisterConcern
                    .Include(d => d.Company)
                    .Where(predicate).ToList();
                return sisterConcerns;
            }
            catch (Exception)
            {
                return new List<SisterConcern>();
            }
        }
    }
}

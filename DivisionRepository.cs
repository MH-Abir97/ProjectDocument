using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;

namespace Pronali.Data.Repositories.Core
{
    public class DivisionRepository : BaseRepository<Division>, IDivisionRepository
    {
        private readonly ApplicationDbContext db;
        public DivisionRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public IEnumerable<Division> GetAllWithRelatedData(Func<Division, bool> predicate)
        {
            try
            {
                List<Division> division = db.Division
                    .Include(d => d.Company)
                    .Include(d => d.SisterConcern)
                    .Where(predicate).ToList();
                return division;
            }
            catch (Exception)
            {
                return new List<Division>();
            }
        }
    }
}

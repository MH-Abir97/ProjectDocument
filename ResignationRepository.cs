using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    class ResignationRepository : BaseRepository<Resignation>, IResignationRepository
    {
        private readonly ApplicationDbContext _db;
        public ResignationRepository(ApplicationDbContext _context) : base(_context)
        {
            _db = _context;
        }
        public List<Resignation> GetAllWithRelatedData(Func<Resignation, bool> predicate)
        {
            return _db.Resignation.Include(r => r.Employee).Where(r => r.IsActive == true).ToList();
        }
    }
}

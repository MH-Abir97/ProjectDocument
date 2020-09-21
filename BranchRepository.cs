using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Core
{
    public class BranchRepository : BaseRepository<Branch>, IBranchRepository
    {
        private readonly ApplicationDbContext db;
        public BranchRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
        public IEnumerable<Branch> GetAllWithRelatedData(Func<Branch, bool> predicate)
        {
            try
            {
                List<Branch> branch = db.Branch
                    .Include(d => d.Company)
                    .Where(predicate).ToList();
                return branch;
            }
            catch (Exception)
            {
                return new List<Branch>();
            }
        }
    }

}

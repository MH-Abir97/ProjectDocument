using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Pronali.Data.Repositories.Hr
{
    public class BranchHeadRepository : BaseRepository<BranchHead>, IBranchHeadRepository
    {
        private readonly ApplicationDbContext db;

        public BranchHeadRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
        public List<BranchHead> GetAllWithRelatedData()
        {
            try
            {
                List<BranchHead> branches = db.BranchHead
                    .Include(d => d.Company)
                    .Include(d => d.Employee)
                    .Include(d => d.Branch)
                    .Include(d => d.Division)
                    .Include(d => d.SisterConcern)
                    .Where(x => x.IsDeleted == false)
                    .ToList();
                return branches;
            }
            catch (Exception)
            {
                return new List<BranchHead>();
            }
        }
    }
}

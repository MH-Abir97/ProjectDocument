using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    class PromotionRepository : BaseRepository<Promotion>, IPromotionRepository
    {
        private readonly ApplicationDbContext db;
        public PromotionRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public IEnumerable<Promotion> GetAllWithRelatedData(Func<Promotion, bool> predicate)
        {
            try
            {
                List<Promotion> promotions = db.Promotion
                    .Include(p => p.Employee)
                    .Include(p => p.PrevBranch)
                    .Include(p => p.TransferedBranch)
                    .Include(p => p.PrevDepartment)
                    .Include(p => p.TransferedDepartment)
                    .Include(p => p.PrevDesignation)
                    .Include(p => p.TransferedDesignation)
                    .Include(p => p.PrevSisterConcern)
                    .Include(p => p.TransferedSisterConcern)
                    .Include(p => p.PrevDivision)
                    .Include(p => p.TransferedDivision)
                    .Where(predicate).ToList();
                return promotions;
            }
            catch (Exception)
            {
                return new List<Promotion>();
            }
        }
    }
}

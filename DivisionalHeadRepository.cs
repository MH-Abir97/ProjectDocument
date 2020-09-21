using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pronali.Data.Repositories.Hr
{
    public class DivisionalHeadRepository: BaseRepository<DivisionalHead>, IDivisionalHeadRepository
    {
        private readonly ApplicationDbContext db;

        public DivisionalHeadRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
        public List<DivisionalHead> GetAllWithRelatedData()
        {
            try
            {
                List<DivisionalHead> divisionalHeads = db.DivisionalHead
                    .Include(d => d.Company)
                    .Include(d => d.Employee)
                    .Include(d => d.Division)
                    .Include(d => d.SisterConcern)
                    .Where(x => x.IsDeleted == false)
                    .ToList();
                return divisionalHeads;
            }
            catch (Exception)
            {
                return new List<DivisionalHead>();
            }
        }
    }
}

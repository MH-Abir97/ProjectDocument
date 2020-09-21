using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pronali.Data.Repositories.Hr
{
    public class DepartmentalHeadRepository: BaseRepository<DepartmentalHead>, IDepartmentalHeadRepository
    {
        private readonly ApplicationDbContext db;

        public DepartmentalHeadRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
        public List<DepartmentalHead> GetAllWithRelatedData()
        {
            try
            {
                List<DepartmentalHead> divisionalHeads = db.DepartmentalHead
                    .Include(d => d.Company)
                    .Include(d => d.Employee)
                    .Include(d => d.Department)
                    .Include(d => d.Branch)
                    .Include(d => d.Division)
                    .Include(d => d.SisterConcern)
                    .Where(x => x.IsDeleted == false)
                    .ToList();
                return divisionalHeads;
            }
            catch (Exception)
            {
                return new List<DepartmentalHead>();
            }
        }
    }
}

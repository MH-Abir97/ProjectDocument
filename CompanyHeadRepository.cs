using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
   public class CompanyHeadRepository: BaseRepository<CompanyHead>, ICompanyHeadRepository
    {
        private readonly ApplicationDbContext db;

        public CompanyHeadRepository(ApplicationDbContext _context) :base(_context)
        {
            db = _context;
        }
        public List<CompanyHead> GetAllWithRelatedData()
        {
            try
            {
                List<CompanyHead> companyHeads = db.CompanyHead
                    .Include(d => d.Company)
                    .Include(d => d.Employee)
                    .Where(x => x.IsDeleted == false)
                    .ToList();
                return companyHeads;
            }
            catch (Exception)
            {
                return new List<CompanyHead>();
            }
        }
    }
}

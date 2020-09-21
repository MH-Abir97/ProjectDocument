using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Core
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext db;
        public CompanyRepository(ApplicationDbContext _context): base(_context)
        {
            db = _context;
        }

        //public IEnumerable<Company> GetAllDefault()
        //{
        //    return _entities.Where(x=>x.IsActive==true && x.IsDeleted==false).ToList();
        //}

        //public Company GetDefault(long id)
        //{
        //    return _entities.FirstOrDefault(x=>x.Id==id && x.IsActive==true);
        //}

        //public Company GetDefault(int id)
        //{
        //    return _entities.Find(id);
        //}
    }

}

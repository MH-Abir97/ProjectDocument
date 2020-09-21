using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
   public class HospitalRepository: BaseRepository<Hospital>, IHospitalRepository
    {
        private readonly ApplicationDbContext db;

        public HospitalRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

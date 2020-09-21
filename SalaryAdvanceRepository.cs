using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    public class SalaryAdvanceRepository : BaseRepository<SalaryAdvance>, ISalaryAdvanceRepository
    {
        private readonly ApplicationDbContext db;
        public SalaryAdvanceRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

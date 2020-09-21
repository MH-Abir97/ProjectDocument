using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    class InvestorRepository:BaseRepository<Investor>,IInvestorRepository
    {
        private readonly ApplicationDbContext db;
        public InvestorRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

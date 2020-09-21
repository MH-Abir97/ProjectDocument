using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    public class AccountSubLedgerRepository : BaseRepository<AccountSubLedger>, IAccountSubLedgerRepository
    {
        private readonly ApplicationDbContext db;
        public AccountSubLedgerRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public List<AccountSubLedger> GetAllWithType()
        {
            return db.AccountSubLedgers
                .Include(x => x.AccountLedger)
                .Include(x => x.AccountLedgerGroup)
                .Where(x => x.IsDeleted == false)
                .ToList();
        }
    }
}

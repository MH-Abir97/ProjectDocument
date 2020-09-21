using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    public class AccountLedgerRepository : BaseRepository<AccountLedger>, IAccountLedgerRepository
    {
        private readonly ApplicationDbContext db;
        public AccountLedgerRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public List<AccountLedger> GetAllWithGroup()
        {
            return db.AccountLedgers
                .Include(x => x.AccountGroup)
                .Where(x => x.IsDeleted == false)
                .ToList();
        }
    }
}

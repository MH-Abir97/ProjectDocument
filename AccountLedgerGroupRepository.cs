using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System.Collections.Generic;
using System.Linq;

namespace Pronali.Data.Repositories.Accounts
{
    public class AccountLedgerGroupRepository : BaseRepository<AccountLedgerGroup>, IAccountLedgerGroupRepository
    {
        private readonly ApplicationDbContext db;
        public AccountLedgerGroupRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public List<AccountLedgerGroup> GetAllWithType()
        {
            return db.AccountLedgerGroups
                .Include(x => x.AccountLedger)
                .Where(x => x.IsDeleted == false)
                .ToList();
        }
    }
}

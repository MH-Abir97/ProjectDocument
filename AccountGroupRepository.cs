using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    public class AccountGroupRepository : BaseRepository<AccountGroup>, IAccountGroupRepository
    {
        private readonly ApplicationDbContext db;
        public AccountGroupRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public List<AccountGroup> GetAllWithType()
        {
            return db.AccountGroups
                .Include(x => x.AccountType)
                .Where(x => x.IsDeleted == false)
                .ToList();
        }
    }
}

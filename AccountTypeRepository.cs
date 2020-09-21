using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    public class AccountTypeRepository : BaseRepository<AccountType>, IAccountTypeRepository
    {
        private readonly ApplicationDbContext db;
        public AccountTypeRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

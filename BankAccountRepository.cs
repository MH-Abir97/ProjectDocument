using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System.Collections.Generic;
using System.Linq;

namespace Pronali.Data.Repositories.Accounts
{
   public class BankAccountRepository: BaseRepository<BankAccount>, IBankAccountRepository
    {
        private readonly ApplicationDbContext db;
        public BankAccountRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

    }
}

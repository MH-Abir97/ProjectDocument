using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System.Collections.Generic;
using System.Linq;

namespace Pronali.Data.Repositories.Accounts
{
    public class LoanReceivableRepository : BaseRepository<LoanReceivable>, ILoanReceivableRepository
    {
        private readonly ApplicationDbContext db;
        public LoanReceivableRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        //public List<LoanReceivable> GetAllWithBankAccountAndEmployee()
        //{
        //    return db.LoanReceivables.Include(x => x.BankAccount).Include(x => x.Employee).ToList();
        //}

        public LoanReceivable GetWithBankAccountAndEmployee(int loanReceivableId)
        {
            return db.LoanReceivables
                .Where(x => x.Id == loanReceivableId)
                .Include(x => x.BankAccount)
                .Include(x => x.Employee)
                .FirstOrDefault();
        }

        public List<LoanReceivable> GetAllWithBankAccountAndEmployee()
        {
            return db.LoanReceivables
                .Where(x => x.IsDeleted == false)
                .Include(x => x.BankAccount)
                .Include(x => x.Employee)
                .OrderByDescending(x => x.Id)
                .ToList();
        }
    }
}

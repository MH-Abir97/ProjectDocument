using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    public class LoanDisbursementRepository: BaseRepository<LoanDisbursement>, ILoanDisbursementRepository
    {
        private readonly ApplicationDbContext _context;

        public LoanDisbursementRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

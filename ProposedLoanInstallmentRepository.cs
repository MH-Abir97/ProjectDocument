using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    public class ProposedLoanInstallmentRepository:BaseRepository<ProposedLoanInstallment>, IProposedLoanInstallmentRepository
    {
        private readonly ApplicationDbContext _context;

        public ProposedLoanInstallmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}

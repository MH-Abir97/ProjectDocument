using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    public class VoucherHeadHistoryRepository : BaseRepository<VoucherHeadHistory>, IVoucherHeadHistoryRepository
    {
        private readonly ApplicationDbContext db;
        public VoucherHeadHistoryRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

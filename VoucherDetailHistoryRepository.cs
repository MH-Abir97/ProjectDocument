using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    public class VoucherDetailHistoryRepository : BaseRepository<VoucherDetailHistory>, IVoucherDetailHistoryRepository
    {
        private readonly ApplicationDbContext db;
        public VoucherDetailHistoryRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

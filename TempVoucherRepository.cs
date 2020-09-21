using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    public class TempVoucherRepository : BaseRepository<TempVoucher>, ITempVoucherRepository
    {
        private readonly ApplicationDbContext db;

        public TempVoucherRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public TempVoucher FindByVoucherType(VoucherTypes vouchertype)
        {
            return db.TempVouchers.Where(x => x.VoucherType == vouchertype).FirstOrDefault();
        }
    }
}

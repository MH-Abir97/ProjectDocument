using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.POS
{
    public class TempVoucherHeadRepository : BaseRepository<TempVoucherHead>, ITempVoucherHeadRepository
    {
        private readonly ApplicationDbContext db;
        public TempVoucherHeadRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public TempVoucherHead GetVoucherTypeWiseUnsavedVoucher(int tempVoucherId)
        {
            return db.TempVoucherHeads.Where(x => x.VoucherId == tempVoucherId && x.VoucherSaveState == VoucherSaveStates.Unsaved).FirstOrDefault();
        }

        public TempVoucherHead GetVoucherTypeWiseLastOrDefault(int tempVoucherId)
        {
            return db.TempVoucherHeads.Where(x => x.VoucherId == tempVoucherId).LastOrDefault();
        }

        public TempVoucherHead GetVoucherNumberWiseVoucher(string voucherNumber)
        {
            return db.TempVoucherHeads.Where(x => x.VoucherNumber == voucherNumber).FirstOrDefault();
        }
    }
}

using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System.Linq;

namespace Pronali.Data.Repositories.Accounts
{
    public class VoucherNumberRepository : BaseRepository<VoucherNumber>, IVoucherNumberRepository
    {
        private readonly ApplicationDbContext db;
        public VoucherNumberRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public VoucherNumber GetVoucherNumberWiseVoucher(string voucherNumber)
        {
            return db.VoucherNumbers.Where(x => x.Number == voucherNumber).FirstOrDefault();
        }

        public VoucherNumber GetVoucherTypeWiseLastOrDefault(int voucherId)
        {
            return db.VoucherNumbers.Where(x => x.VoucherId == voucherId).LastOrDefault();
        }

        public VoucherNumber GetVoucherTypeWiseUnsavedVoucher(int voucherId)
        {
            return db.VoucherNumbers.Where(x => x.VoucherId == voucherId && x.State == VoucherSaveStates.Unsaved).LastOrDefault();
        }
    }
}

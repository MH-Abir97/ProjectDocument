using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System.Linq;

namespace Pronali.Data.Repositories.Accounts
{
    public class VoucherRepository : BaseRepository<Voucher>, IVoucherRepository
    {
        private readonly ApplicationDbContext db;
        public VoucherRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public Voucher FindByVoucherType(VoucherTypes vouchertype)
        {
            return db.Vouchers.Where(x => x.VoucherType == vouchertype).FirstOrDefault();
        }
    }
}

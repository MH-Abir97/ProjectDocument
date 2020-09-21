using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System.Linq;

namespace Pronali.Data.Repositories.Accounts
{
    public class VoucherDetailRepository : BaseRepository<VoucherDetail>, IVoucherDetailRepository
    {
        private readonly ApplicationDbContext db;

        public VoucherDetailRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public VoucherDetail GetLastOrDefault()
        {
            return db.VoucherDetails.LastOrDefault();
        }
    }
}

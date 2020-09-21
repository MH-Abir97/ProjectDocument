using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System.Linq;

namespace Pronali.Data.Repositories.Accounts
{
    public class TempVoucherDetailRepository : BaseRepository<TempVoucherDetail>, ITempVoucherDetailRepository
    {
        private readonly ApplicationDbContext db;

        public TempVoucherDetailRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public TempVoucherDetail GetLastOrDefault()
        {
            return db.TempVoucherDetails.LastOrDefault();
        }

    }
}

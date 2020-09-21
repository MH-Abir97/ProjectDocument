using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class TelephoneAndMobileRepository: BaseRepository<TelephoneAndMobile>, ITelephoneAndMobileRepository
    {
        private readonly ApplicationDbContext db;
        public TelephoneAndMobileRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

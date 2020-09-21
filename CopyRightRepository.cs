using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
   public class CopyRightRepository: BaseRepository<CopyRight>, ICopyRightRepository
    {
        private readonly ApplicationDbContext db;
        public CopyRightRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

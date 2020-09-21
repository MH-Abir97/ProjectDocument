using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
   public class OfficeDecorationRepository: BaseRepository<OfficeDecoration>, IOfficeDecorationRepository
    {
        private readonly ApplicationDbContext db;
        public OfficeDecorationRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

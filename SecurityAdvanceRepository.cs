using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
   public class SecurityAdvanceRepository: BaseRepository<SecurityAdvance>, ISecurityAdvanceRepository
    {
        private readonly ApplicationDbContext db;
        public SecurityAdvanceRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class AdvanceForWorkRepository: BaseRepository<AdvanceForWork>, IAdvanceForWorkRepository
    {
        private readonly ApplicationDbContext db;
        public AdvanceForWorkRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

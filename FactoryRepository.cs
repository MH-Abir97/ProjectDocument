using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class FactoryRepository:BaseRepository<Factory>, IFactoryRepository
    {
        private readonly ApplicationDbContext db;
        public FactoryRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

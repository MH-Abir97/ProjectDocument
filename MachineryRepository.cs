using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class MachineryRepository:BaseRepository<Machinery>, IMachineryRepository
    {
        private readonly ApplicationDbContext db;
        public MachineryRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
   public class SoftwareRepository: BaseRepository<Software>, ISoftwareRepository
    {
        private readonly ApplicationDbContext db;
        public SoftwareRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

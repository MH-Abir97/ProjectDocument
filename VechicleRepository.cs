using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class VechicleRepository: BaseRepository<Vechicle>, IVechicleRepository
    {
        private readonly ApplicationDbContext db;
        public VechicleRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class LaptopAndTabRepository: BaseRepository<LaptopAndTab>, ILaptopAndTabRepository
    {
        private readonly ApplicationDbContext db;
        public LaptopAndTabRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

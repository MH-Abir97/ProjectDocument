using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class ComputerAndAccessoriesRepository: BaseRepository<ComputerAndAccessories>, IComputerAndAccessoriesRepository
    {
        private readonly ApplicationDbContext db;
        public ComputerAndAccessoriesRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

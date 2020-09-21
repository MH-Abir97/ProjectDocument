using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class FurnitureAndFixtureRepository: BaseRepository<FurnitureAndFixture>, IFurnitureAndFixtureRepository
    {
        private readonly ApplicationDbContext db;
        public FurnitureAndFixtureRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

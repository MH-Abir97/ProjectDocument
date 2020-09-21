using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class BuildingRepository: BaseRepository<Building>, IBuildingRepository
    {
        private readonly ApplicationDbContext db;
        public BuildingRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

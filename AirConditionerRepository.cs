using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class AirConditionerRepository: BaseRepository<AirConditioner>, IAirConditionerRepository
    {
        private readonly ApplicationDbContext db;
        public AirConditionerRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

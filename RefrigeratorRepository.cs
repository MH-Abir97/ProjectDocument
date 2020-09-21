using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class RefrigeratorRepository:BaseRepository<Refrigerator>, IRefrigeratorRepository
    {
        private readonly ApplicationDbContext db;
        public RefrigeratorRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

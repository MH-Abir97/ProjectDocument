using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
  public class EquipmentRepository:BaseRepository<Equipment>, IEquipmentRepository
    {
        private readonly ApplicationDbContext db;
        public EquipmentRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

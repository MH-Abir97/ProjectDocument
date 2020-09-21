using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    public class RoasterGroupDetailsRepository : BaseRepository<RoasterGroupDetails>, IRoasterGroupDetailsRepository
    {
        private readonly ApplicationDbContext db;
        public RoasterGroupDetailsRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

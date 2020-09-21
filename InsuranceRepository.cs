using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    class InsuranceRepository: BaseRepository<Insurance>, IInsuranceRepository
    {
        readonly ApplicationDbContext _db;
        public InsuranceRepository(ApplicationDbContext _context) : base(_context)
        {
            _db = _context;
        }
    }
}

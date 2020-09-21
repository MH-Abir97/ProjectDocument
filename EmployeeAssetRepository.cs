using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    class EmployeeAssetRepository : BaseRepository<EmployeeAsset>, IEmployeeAssetRepository
    {
        private ApplicationDbContext _db;
        public EmployeeAssetRepository(ApplicationDbContext _context) : base(_context)
        {
            _db = _context;
        }
    }
}

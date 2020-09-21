using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    class EmployeeIncomeTaxRepository : BaseRepository<EmployeeIncomeTax>, IEmployeeIncomeTaxRepository
    {
        readonly ApplicationDbContext _db;
        public EmployeeIncomeTaxRepository(ApplicationDbContext _context) : base(_context)
        {
            _db = _context;
        }
    }
}

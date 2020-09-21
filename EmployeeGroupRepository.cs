using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;

namespace Pronali.Data.Repositories.Hr
{
    public class EmployeeGroupRepository : BaseRepository<EmployeeGroup>, IEmployeeGroupRepository
    {
        private readonly ApplicationDbContext db;
        public EmployeeGroupRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

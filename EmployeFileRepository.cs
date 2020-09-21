using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
   public class EmployeFileRepository: BaseRepository<EmployeFile>, IEmployeFileRepository
    {
        private readonly ApplicationDbContext db;
        public EmployeFileRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

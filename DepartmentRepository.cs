using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Core
{
   public class DepartmentRepository:BaseRepository<Department>,IDepartmentRepository
    {
        private readonly ApplicationDbContext db;
        public DepartmentRepository(ApplicationDbContext _context):base(_context)
        {
            db = _context;
        }
    }

}

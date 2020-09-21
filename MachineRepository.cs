using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Core
{
   public class MachineRepository:BaseRepository<Machine>, IMachineRepository
    {
        private ApplicationDbContext _db;
        public MachineRepository(ApplicationDbContext _context):base(_context)
        {

        }
    }
}

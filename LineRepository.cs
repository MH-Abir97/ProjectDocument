using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Core
{
   public class LineRepository:BaseRepository<Line>,ILineRepository
    {
        private ApplicationDbContext _db;
        public LineRepository(ApplicationDbContext _context):base(_context)
        {

        }
    }
}

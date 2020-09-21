using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;

namespace Pronali.Data.Repositories.Hr
{
    public class DataProcessQueueRepository : BaseRepository<DataProcessQueue>, IDataProcessQueueRepository
    {
        private readonly ApplicationDbContext db;
        public DataProcessQueueRepository(ApplicationDbContext _context) : base(_context)
        {
            this.db = _context;
        }
    }
}

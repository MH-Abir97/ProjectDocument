using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;

namespace Pronali.Data.Repositories.Hr
{
    public class JobLocationRepository : BaseRepository<JobLocation>, IJobLocationRepository
    {
        private readonly ApplicationDbContext db;
        public JobLocationRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

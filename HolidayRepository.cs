using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    class HolidayRepository : BaseRepository<Holiday>, IHolidayRepository
    {
        private readonly ApplicationDbContext db;
        public HolidayRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

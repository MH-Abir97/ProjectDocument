using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    public class LeaveRepository : BaseRepository<Leave>, ILeaveRepository
    {
        private readonly ApplicationDbContext db;
        public LeaveRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

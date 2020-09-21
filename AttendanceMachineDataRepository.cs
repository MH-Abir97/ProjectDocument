using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;

namespace Pronali.Data.Repositories.Hr
{
    public class AttendanceMachineDataRepository : BaseRepository<AttendanceMachineData>, IAttendanceMachineDataRepository
    {
        private readonly ApplicationDbContext db;
        public AttendanceMachineDataRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

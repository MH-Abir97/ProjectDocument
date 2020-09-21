using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;

namespace Pronali.Data.Repositories.Hr
{
    public class AttendanceProcessedDataRepository : BaseRepository<AttendanceProcessedData>, IAttendanceProcessedDataRepository
    {
        private readonly ApplicationDbContext db;
        public AttendanceProcessedDataRepository(ApplicationDbContext _context) : base(_context)
        {
            this.db = _context;
        }
    }
}

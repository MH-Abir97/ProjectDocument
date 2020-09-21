using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;

namespace Pronali.Data.Repositories.Hr
{
    public class AttendanceMachineDataFilteredRepository : BaseRepository<AttendanceMachineDataFiltered>, IAttendanceMachineDataFilteredRepository
    {
        private readonly ApplicationDbContext db;
        public AttendanceMachineDataFilteredRepository(ApplicationDbContext _context) : base(_context)
        {
            this.db = _context;
        }
    }
}

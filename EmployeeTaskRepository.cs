using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
   public class EmployeeTaskRepository:BaseRepository<EmployeeTask>, IEmployeeTaskRepository
    {
        private readonly ApplicationDbContext db;
        public EmployeeTaskRepository(ApplicationDbContext _context) : base(_context)
        {
            this.db = _context;
        }

        public IEnumerable<EmployeeTask> GetWithAllData()
        {
            return db.EmployeeTask.Include(e => e.Assignee).Include(e => e.AssignTo);
        }
    }
}

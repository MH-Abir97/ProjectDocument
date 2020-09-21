using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces;
using Pronali.Data.Repositories.Interfaces.Hr;

namespace Pronali.Data.Repositories.Hr
{
    public class EmployeeWeekendRepository : BaseRepository<EmployeeWeekend>, IEmployeeWeekendRepository
    {
        private readonly ApplicationDbContext db;
        public EmployeeWeekendRepository(ApplicationDbContext _dbContext) : base(_dbContext)
        {
            db = _dbContext;
        }

        public IEnumerable<EmployeeWeekend> GetAllEmployeeAndWeekend()
        {
            return db.EmployeeWeekend.Include(x => x.Employee).ToList();
        }

        public IEnumerable<EmployeeWeekend> GetAllWithRelatedData(Func<EmployeeWeekend, bool> p)
        {
            return db.EmployeeWeekend.Include(e => e.Employee).Where(p);
        }
    }
}

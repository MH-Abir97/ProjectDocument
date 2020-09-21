using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Hr
{
    public class EmployeeLeaveRepository : BaseRepository<EmployeeLeave>, IEmployeeLeaveRepository
    {
        private readonly ApplicationDbContext db;
        public EmployeeLeaveRepository(ApplicationDbContext _dbContext) : base(_dbContext)
        {
            db = _dbContext;
        }

        public List<EmployeeLeave> FindWithRelatedData(Func<EmployeeLeave, bool> predicate)
        {
            return _entities.Include(c => c.Leave)
                .Where(predicate).ToList();
        }
    }
}

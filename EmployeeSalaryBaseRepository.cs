using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Repositories.Interfaces.Hr;

namespace Pronali.Data.Repositories.Hr
{
    public class EmployeeSalaryBaseRepository : BaseRepository<EmployeeSalaryBase>, IEmployeeSalaryBaseRepository
    {
        private readonly ApplicationDbContext db;
        public EmployeeSalaryBaseRepository(ApplicationDbContext _dbContext) : base(_dbContext)
        {
            db = _dbContext;
        }
    }
}

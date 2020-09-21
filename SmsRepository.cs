using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Core
{
    public class SmsRepository : BaseRepository<SMS>, ISmsRepository
    {
        private readonly ApplicationDbContext db;

        public SmsRepository(ApplicationDbContext _dbContext) : base(_dbContext)
        {
            db = _dbContext;
        }
    }
}

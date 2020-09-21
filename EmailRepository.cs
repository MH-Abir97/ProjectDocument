using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Core
{
    public class EmailRepository : BaseRepository<Email>, IEmailRepository
    {
        private readonly ApplicationDbContext db;
        public EmailRepository(ApplicationDbContext _dbContext) :base(_dbContext)
        {
            db = _dbContext;
        }
    }
}

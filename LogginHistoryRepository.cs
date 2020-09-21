using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Core
{
   public class LogginHistoryRepository:BaseRepository<LoginHistory>, ILoginHistoryRepository
    {
        private readonly ApplicationDbContext db;
        public LogginHistoryRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public IEnumerable<LoginHistory> GetAllUserData()
        {
            return db.LoginHistory.Include(x => x.UserId).ToList();
        }
    }
}

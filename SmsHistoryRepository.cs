using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Core
{
   public class SmsHistoryRepository:BaseRepository<SmsHistory>, ISmsHistoryRepository
    {
        private readonly ApplicationDbContext db;
        public SmsHistoryRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

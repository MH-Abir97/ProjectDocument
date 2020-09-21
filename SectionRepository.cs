using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Core
{
   public class SectionRepository : BaseRepository<Section>, ISectionRepository
    {
        private readonly ApplicationDbContext db;
        public SectionRepository(ApplicationDbContext _dbcontext) : base(_dbcontext)
        {
            db = _dbcontext;
        }
    }
}

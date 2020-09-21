using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Repositories.Interfaces.Core;

namespace Pronali.Data.Repositories.Core
{
    public class SystemPreferenceRepository : BaseRepository<SystemPreference>, ISystemPreferenceRepository
    {
        private readonly ApplicationDbContext db;

        public SystemPreferenceRepository(ApplicationDbContext _dbContext) : base(_dbContext)
        {
            db = _dbContext;
        }
    }
}

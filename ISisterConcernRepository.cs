using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Core;

namespace Pronali.Data.Repositories.Interfaces.Core
{
    public interface ISisterConcernRepository : IBaseRepository<SisterConcern>
    {
        IEnumerable<SisterConcern> GetAllWithRelatedData(Func<SisterConcern, bool> predicate);
    }
}

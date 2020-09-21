using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Core;

namespace Pronali.Data.Repositories.Interfaces.Core
{
    public interface IDivisionRepository : IBaseRepository<Division>
    {
        IEnumerable<Division> GetAllWithRelatedData(Func<Division, bool> predicate);
    }
}

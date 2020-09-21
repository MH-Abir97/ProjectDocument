using Pronali.Data.Models.Entity.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Core
{
    public interface IBranchRepository : IBaseRepository<Branch>
    {
        IEnumerable<Branch> GetAllWithRelatedData(Func<Branch, bool> predicate);
    }
}

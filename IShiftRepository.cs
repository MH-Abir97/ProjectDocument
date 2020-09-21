using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IShiftRepository : IBaseRepository<Shift>
    {
        Shift GetFirstOrDefaultWithRelatedData(Func<Shift, bool> predicate);
        IEnumerable<Shift> FindWithRelatedData();
    }
}

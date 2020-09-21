using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IShortLeaveApplicationRepository : IBaseRepository<ShortLeaveApplication>
    {
        ShortLeaveApplication GetFirstOrDefaultwithRelatedData(Func<ShortLeaveApplication, bool> predicate);
        IEnumerable<ShortLeaveApplication> FindWithRelatedData(Func<ShortLeaveApplication, bool> predicate);
    }
}

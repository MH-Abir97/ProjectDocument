using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IEarlyOutPermissionRepository : IBaseRepository<EarlyOutPermission>
    {
        List<EarlyOutPermission> FindWithRelatedData(Func<EarlyOutPermission, bool> predicate);
        EarlyOutPermission GetFirstOrDefaultwithRelatedData(Func<EarlyOutPermission, bool> predicate);
    }
}

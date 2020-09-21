using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface ILatePermissionRepository : IBaseRepository<LatePermission>
    {
        List<LatePermission> FindWithRelatedData(Func<LatePermission, bool> predicate);
        LatePermission GetFirstOrDefaultwithRelatedData(Func<LatePermission, bool> predicate);
    }
}

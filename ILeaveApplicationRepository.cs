using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface ILeaveApplicationRepository : IBaseRepository<LeaveApplication>
    {
        IEnumerable<LeaveApplication> FindWithRelatedData(Func<LeaveApplication, bool> predicate);
        LeaveApplication GetFirstOrDefaultwithRelatedData(Func<LeaveApplication, bool> predicate);
    }
}

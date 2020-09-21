using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Core;

namespace Pronali.Data.Repositories.Interfaces.Core
{
    public interface IApplicationApprovalRepository : IBaseRepository<ApplicationApproval>
    {
        

        void SetApprover(ApplicationType applicationType,long employeeId,string id, DateTime time, string createdby);
        List<ApplicationApproval> GetWithAllEmployeeData();
    }
}

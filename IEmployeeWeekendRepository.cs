using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IEmployeeWeekendRepository : IBaseRepository<EmployeeWeekend>
    {
        IEnumerable<EmployeeWeekend> GetAllEmployeeAndWeekend();

        IEnumerable<EmployeeWeekend> GetAllWithRelatedData(Func<EmployeeWeekend, bool> p);
    }
}

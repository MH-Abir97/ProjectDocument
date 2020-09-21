using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Employee GetFirstOrDefaultWithRelatedData(Func<Employee, bool> predicate);
        List<Employee> GetAllWithRelatedData(Func<Employee, bool> predicate);
       
    }
}

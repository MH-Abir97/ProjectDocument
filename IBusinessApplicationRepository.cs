using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IBusinessApplicationRepository : IBaseRepository<BusinessApplication>
    {
        List<BusinessApplication> FindWithRelatedData(Func<BusinessApplication, bool> predicate);
        BusinessApplication GetFirstOrDefaultwithRelatedData(Func<BusinessApplication, bool> predicate);
    }
}

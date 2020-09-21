using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IShortBusinessApplicationRepository : IBaseRepository<ShortBusinessApplication>
    {
        List<ShortBusinessApplication> FindWithRelatedData(Func<ShortBusinessApplication, bool> predicate);
        ShortBusinessApplication GetFirstOrDefaultwithRelatedData(Func<ShortBusinessApplication, bool> predicate);
    }
}

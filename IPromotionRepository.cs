using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IPromotionRepository : IBaseRepository<Promotion>
    {
        IEnumerable<Promotion> GetAllWithRelatedData(Func<Promotion, bool> predicate);
    }
}

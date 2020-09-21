using Pronali.Data.Models.Entity.Hr;
using System.Collections.Generic;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IDepartmentalHeadRepository: IBaseRepository<DepartmentalHead>
    {
        List<DepartmentalHead> GetAllWithRelatedData();
    }
}

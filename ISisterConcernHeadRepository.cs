using Pronali.Data.Models.Entity.Hr;
using System.Collections.Generic;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface ISisterConcernHeadRepository: IBaseRepository<SisterConcernHead>
    {
        List<SisterConcernHead> GetAllWithRelatedData();
    }
}

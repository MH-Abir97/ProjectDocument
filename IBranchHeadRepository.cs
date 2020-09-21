using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections.Generic;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IBranchHeadRepository: IBaseRepository<BranchHead>
    {
        List<BranchHead> GetAllWithRelatedData();
    }
}

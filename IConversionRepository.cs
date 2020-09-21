using Pronali.Data.Models.Entity.POS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.POS
{
    public interface IConversionRepository : IBaseRepository<Conversion>
    {
        Conversion GetWithUnit(int conversionId);
        List<Conversion> GetAllWithUnit();
    }
}

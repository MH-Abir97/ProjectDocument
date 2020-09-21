using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
   public interface IManualAttendanceRepository: IBaseRepository<ManualAttendance>
    {
        List<ManualAttendance> GetAllWithRelatedData();
        ManualAttendance GetFirstOrDefaultwithRelatedData(Func<ManualAttendance, bool> predicate);
    }
}

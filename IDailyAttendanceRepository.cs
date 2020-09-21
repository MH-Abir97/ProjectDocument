using System;
using System.Collections.Generic;
using System.Text;
using Pronali.Data.Models.Entity.Hr;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
    public interface IDailyAttendanceRepository : IBaseRepository<DailyAttendance>
    {
        List<DailyAttendance> GetAllWithRelatedData(Func<DailyAttendance, bool> predicate);
    }
}

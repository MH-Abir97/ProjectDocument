using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Pronali.Data.Enum
{
    public enum ApplicationType
    {
        [Description("Leave Application")]
        Leave =100,
        [Description("Business Application")]
        BusinessTravel = 101,
        Early=102,
        Late=103,
        Roster=104,
        ManualAbsent=105,
        ManualPresent=106,
        SalarySheet=107,
        Performance=108,
        Promotion=109,
        Loan=110,
        Employee=111,
        User=112,
        Shift=113,
        Retirement=114,
        Resign=115,
        ShortLeave=116,
        ShortBusinessTravel=117

    }
}

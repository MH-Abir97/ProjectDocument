using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Pronali.Data.Enum
{
    public enum AttendanceFlag
    {
        [Description("PRESENT")]
        P=1,  // PRESENT
        [Description("ABSENT")]
        A=0,  // ABSENT
        [Description("LATE IN")]
        L=3,  // LATE IN
        [Description("EARLY OUT")]
        E=2,   // EARLY OUT
        [Description("LATE PERMISSION")]
        LP=6,   // LATE PERMISSION
        [Description("EARLY OUT PERMISSION")]
        EP=7,   // EARLY OUT PERMISSION
        [Description("LEAVE")]
        LE=4,   // LEAVE
        [Description("BUSINESS TRAVEL")]
        BT=5,   // BUSINESS TRAVEL
        [Description("HOLIDAY")]
        H=8,   // HOLIDAY
        [Description("WEEKEND")]
        W=9,   // WEEKEND
        [Description("Special Case")]
        SP = 10,   // Special Case
        [Description("Manual Present")]
        MP = 11,   // Manual Present
        [Description("Manual Absent")]
        MA = 12,   // Manual Absent
        [Description("Partial Leave")]
        PL = 13,   // Partial Leave
        [Description("Partial Business Travel")]
        PB = 14,   // Partial Business Travel
    }
}

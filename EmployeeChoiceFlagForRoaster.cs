using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Pronali.Data.Enum
{
    public enum EmployeeChoiceFlagForRoaster
    {
        [Description("All Employee In Shift")]
        AllEmployeeInShift,
        [Description("Selected Roaster Employee")]
        SelectedRoasterEmployee,
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pronali.Data.Enum
{
    public enum OverTimeEnum
    {
        Quarter = 15,
        Half = 30,
        [Display(Name = "Three Quarter")]
        ThreeQuarter = 45,
        Full = 60
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pronali.Data.Enum
{
    public enum ApprovalLevel
    {
        [Display(Name = "Level 1")]
        Level_1 = 1,
        [Display(Name = "Level 2")]
        Level_2 = 2,
        [Display(Name = "Level 3")]
        Level_3 = 3,
        [Display(Name = "Level 4")]
        Level_4 = 4,
        [Display(Name = "Level 5")]
        Level_5 = 5,
        [Display(Name = "Level 6")]
        Level_6 = 6
    }
}

﻿using Pronali.Data.Models.Entity.Hr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.Hr
{
   public interface ICompanyHeadRepository: IBaseRepository<CompanyHead>
    {
        List<CompanyHead> GetAllWithRelatedData();
    }
}

﻿using Pronali.Data.Models.Entity.POS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.POS
{
    public interface IProductPriceRepository : IBaseRepository<ProductPrice>
    {
        ProductPrice GetWithProductAndPrice(int productPriceId);
        List<ProductPrice> GetAllWithProductAndPrice();
    }
}

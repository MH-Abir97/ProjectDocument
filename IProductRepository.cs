using Pronali.Data.Models.Entity.POS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Interfaces.POS
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Product GetWithCategoryAndGroupAndConversion(int productId);
        List<Product> GetAllWithCategoryAndGroupAndConversion();
    }
}

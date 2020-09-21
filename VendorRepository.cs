using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    public class VendorRepository : BaseRepository<Vendor>, IVendorRepository
    {
        private readonly ApplicationDbContext db;

        public VendorRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

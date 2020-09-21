using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pronali.Data.Repositories.Accounts
{
    public class VoucherHeadRepository : BaseRepository<VoucherHead>, IVoucherHeadRepository
    {
        private readonly ApplicationDbContext db;

        public VoucherHeadRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }

        public VoucherHead GetAllWithDetails(string voucherNumber)
        {
            return db.VoucherHeads.Where(x => x.VoucherNumber == voucherNumber).Include(x => x.VoucherDetails).FirstOrDefault();
        }
    }
}

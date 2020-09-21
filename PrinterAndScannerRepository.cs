using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Repositories.Interfaces.Accounts;

namespace Pronali.Data.Repositories.Accounts
{
    public class PrinterAndScannerRepository: BaseRepository<PrinterAndScanner>, IPrinterAndScannerRepository
    {
        private readonly ApplicationDbContext db;
        public PrinterAndScannerRepository(ApplicationDbContext _context) : base(_context)
        {
            db = _context;
        }
    }
}

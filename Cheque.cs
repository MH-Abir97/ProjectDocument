using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Models.Entity.POS;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Cheque", Schema = "Accounts")]
    public class Cheque : BaseModel
    {
        public int Id { get; set; }
        public string ChequeNo { get; set; }
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public string PayableBankName { get; set; }
        public string PayableBankBranch { get; set; }
        public decimal Amount { get; set; }
        public bool IsChequeDeposit { get; set; }
        public bool IsChequeHonor { get; set; }
        public bool IsChequeDishonor { get; set; }
        public bool IsChequeReturn { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public string ChequePhoto { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
    }
}

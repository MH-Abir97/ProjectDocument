using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("AccountPayable", Schema = "Accounts")]
    public class AccountPayable : BaseModel
    {
        public int Id { get; set; }
        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }
        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        [Display(Name = "Account Ledger")]
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        [Display(Name = "Transaction No.")]
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        [Display(Name = "Bank Account")]
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }
        public string Particulars { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PdcDebitAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PdcCreditAmout { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
    }
}

using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("AdvanceForWork", Schema = "Accounts")]
    public class AdvanceForWork : BaseModel
    {
        public int Id { get; set; }
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public int AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Particulars { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PdcDebitAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PdcCreditAmout { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
    }
}

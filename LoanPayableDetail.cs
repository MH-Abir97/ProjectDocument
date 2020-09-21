using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("LoanPayableDetail", Schema = "Accounts")]
    public class LoanPayableDetail : BaseModel
    {
        public int Id { get; set; }
        public int? LoanPayableId { get; set; }
        public LoanPayable LoanPayable { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Particulars { get; set; }
        public string Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
        public decimal PdcDebitAmount { get; set; }
        public decimal PdcCreditAmout { get; set; }
    }
}

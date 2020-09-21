using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("MarketableSecurity", Schema = "Accounts")]
    public class MarketableSecurity : BaseModel
    {
        public int Id { get; set; }
        public int? InvestorId { get; set; }
        public Investor Investor { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Particuler { get; set; }
        public string Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
    }
}

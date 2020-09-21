using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("MarketingExpensessDetail", Schema = "Accounts")]
    public class MarketingExpensessDetail : BaseModel
    {
        public int Id { get; set; }
        public int? MarketingExpenseId { get; set; }
        public MarketingExpense MarketingExpense { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Particuler { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }
}

using Pronali.Data.Models.Entity.POS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("CorporateReputation", Schema = "Accounts")]
    public class CorporateReputation : BaseModel
    {
        public int Id { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public string Particulars { get; set; }
        public string Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
    }
}

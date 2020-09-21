using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("CashInHandDetail", Schema = "Accounts")]
    public class CashInHandDetail : BaseModel
    {
        public int Id { get; set; }
        public int? CashInHandId { get; set; }
        public CashInHand CashInHand { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Particulars { get; set; }
        public string Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
    }
}

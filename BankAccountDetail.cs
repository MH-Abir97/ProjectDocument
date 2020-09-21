using Pronali.Data.Models.Entity.POS;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("BankAccountDetail", Schema = "Accounts")]
    public class BankAccountDetail : BaseModel
    {
        public int Id { get; set; }
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int? ChequeId { get; set; }
        public Cheque Cheque { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PdcAmountDebit { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PdcAmountCredit { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditAmount { get; set; }
        public string Particulars { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
    }
}

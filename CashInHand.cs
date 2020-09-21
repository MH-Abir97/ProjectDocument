using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("CashInHand", Schema = "Accounts")]
    public class CashInHand : BaseModel
    {
        public int Id { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
    }
}

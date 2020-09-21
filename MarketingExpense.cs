using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("MarketingExpense", Schema = "Accounts")]
    public class MarketingExpense : BaseModel
    {
        public int Id { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public decimal Balance { get; set; }
        public string BlanceRemark { get; set; }
    }
}

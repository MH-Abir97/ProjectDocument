using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("OperatingExpense", Schema = "Accounts")]
    public class OperatingExpense : BaseModel
    {
        public int Id { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public string OperatingExpenseName { get; set; }
        public string Remark { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
    }
}

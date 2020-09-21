using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("BankAccount", Schema = "Accounts")]
    public class BankAccount : BaseModel
    {
        public int Id { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public string AccountName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string AccountType { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PdcBalance { get; set; }
        public string PdcBalanceRemark { get; set; }
    }
}

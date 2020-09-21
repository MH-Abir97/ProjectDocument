using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("AccountLedgerGroup", Schema = "Accounts")]
    public class AccountLedgerGroup : BaseModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Account Ledger Group")]
        [StringLength(50)]
        public string AccountLedgerGroupName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Display(Name = "Account Ledger")]
        public int AccountLedgerId { get; set; }
        [ForeignKey("AccountLedgerId")]
        public AccountLedger AccountLedger { get; set; }
    }
}

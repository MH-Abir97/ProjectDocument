using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("AccountSubLedger", Schema = "Accounts")]
    public class AccountSubLedger : BaseModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Account Sub Ledger")]
        [StringLength(50)]
        public string AccountSubLedgerName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Required]
        [Display(Name = "Account Ledger")]
        public int AccountLedgerId { get; set; }
        [ForeignKey("AccountLedgerId")]
        public AccountLedger AccountLedger { get; set; }

        [Display(Name = "Account Ledger Group")]
        public int? AccountLedgerGroupId { get; set; }
        [ForeignKey("AccountLedgerGroupId")]
        public AccountLedgerGroup AccountLedgerGroup { get; set; }

        [Required]
        [Column(TypeName = "varchar(30)")]
        public string DbTrackId { get; set; }
    }
}

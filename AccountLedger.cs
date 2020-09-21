using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("AccountLedger", Schema = "Accounts")]
    public class AccountLedger : BaseModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Account Ledger")]
        [StringLength(50)]
        public string AccountLedgerName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Required]
        [Display(Name = "Tracking Number")]
        [Column(TypeName = "varchar(30)")]
        public string TrackingId { get; set; }

        [Display(Name = "Account Group")]
        public int AccountGroupId { get; set; }
        [ForeignKey("AccountGroupId")]
        public AccountGroup AccountGroup { get; set; }
    }
}

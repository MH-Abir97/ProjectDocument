using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("AccountGroup", Schema = "Accounts")]
    public class AccountGroup : BaseModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Account Group")]
        [StringLength(50)]
        public string AccountGroupName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Required]
        [Display(Name = "Tracking Number")]
        [Column(TypeName = "varchar(30)")]
        public string TrackingId { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public int AccountTypeId { get; set; }
        public AccountType AccountType { get; set; }
    }
}

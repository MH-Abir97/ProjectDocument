using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("AccountType", Schema = "Accounts")]
    public class AccountType : BaseModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        [StringLength(50)]
        public string AccountTypeName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Required]
        [Display(Name = "Tracking No.")]
        [Column(TypeName = "varchar(30)")]
        public string TrackingId { get; set; }
    }
}

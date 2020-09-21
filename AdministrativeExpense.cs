using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("AdministrativeExpense", Schema = "Accounts")]
    public class AdministrativeExpense : BaseModel
    {
        public int Id { get; set; }
        public int AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        public string BlanceRemark { get; set; }
    }
}

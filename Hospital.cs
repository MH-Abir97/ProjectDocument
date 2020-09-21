using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Hospital", Schema = "Accounts")]
    public class Hospital : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HospitalBranch { get; set; }
        public string Address { get; set; }
        public string LandPhone { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string WebAddress { get; set; }
        public string Description { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
        public bool HasCommission { get; set; }
        public double CommissionPercent { get; set; }
        public double CommissionAmount { get; set; }
    }
}

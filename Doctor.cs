using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Doctor", Schema = "Accounts")]
    public class Doctor : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Specialist { get; set; }
        public string HospitalName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string LandPhone { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
        public bool HasCommission { get; set; }
        public double CommissionPercent { get; set; }
        public double CommissionAmount { get; set; }
    }
}

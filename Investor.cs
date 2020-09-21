using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Investor", Schema = "Accounts")]
    public class Investor : BaseModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string InvestorName { get; set; }
        public string Address { get; set; }
        public string LandPhone { get; set; }
        public string Mobile { get; set; }
        public decimal Balance { get; set; }
        public string Email { get; set; }
        public string BalanceRemark { get; set; }
        public decimal PdcBalance { get; set; }
        public string PdcBalaceRemark { get; set; }
        public int? AccountLedgerGroupId { get; set; }
        public AccountLedgerGroup AccountLedgerGroup { get; set; }
        public bool HasCommission { get; set; }
        public double CommissionPercent { get; set; }
        public double CommissionAmount { get; set; }
    }
}

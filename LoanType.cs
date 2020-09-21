using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("LoanType", Schema = "Accounts")]
    public class LoanType : BaseModel
    {
        public int Id { get; set; }
        public string LoanName { get; set; }
        public string Particulars { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
        public bool IsInterestApplicable { get; set; }
        public bool IsCompoundInterest { get; set; }
        public bool IsSimpleInterest { get; set; }
        public int? InterestCalculationId { get; set; }
        public  InterestCalculation InterestCalculation { get; set; }
    }
}

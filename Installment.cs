using Pronali.Data.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Installment", Schema = "Accounts")]
    public class Installment : BaseModel
    {
        public int Id { get; set; }
        public string MethodName { get; set; }
        public decimal PrincipalAmount { get; set; }
        public int NumberOfMonth { get; set; }
        public double InterestRate { get; set; }
        public InterestCalculationMethod InterestCalculationMethod { get; set; }
        public bool IsCompoundInterest { get; set; }
        public bool IsSimpleInterest { get; set; }
        public int NumberOfInstallment { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal NetPaybleAmount { get; set; }
    }
}

using Pronali.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("LoanPayable", Schema = "Accounts")]
    public class LoanPayable : BaseModel
    {
        public int Id { get; set; }
        [Display(Name = "Company")]
        [StringLength(50)]
        public string CompanyName { get; set; }
        [StringLength(50)]
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public string LandPhone { get; set; }
        public string Mobile { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
        public decimal PdcBalance { get; set; }
        public string PdcBalaceRemark { get; set; }
        public InterestCalculationMethod InterestCalculationMethod { get; set; }
        public bool IsInstallment { get; set; }
        public int? IntstallmentId { get; set; }
        public Installment Installment { get; set; }
        public bool IsInterestApplicable { get; set; }
        public bool IsCompoundInterest { get; set; }
        public bool IsSimpleInterest { get; set; }
        public int? AccountLedgerGroupId { get; set; }
        public AccountLedgerGroup AccountLedgerGroup { get; set; }
    }
}

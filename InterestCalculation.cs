using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("InterestCalculation", Schema = "Accounts")]
    public class InterestCalculation : BaseModel
    {
        public int Id { get; set; }
        public LoanType LoanType { get; set; }
        public string InerestType { get; set; }
        public DateTime CalculationTime { get; set; }
    }
}

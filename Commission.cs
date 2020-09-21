using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Models.Entity.POS;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Commission", Schema = "Accounts")]
    public class Commission : BaseModel
    {
        public int Id { get; set; }
        public int? DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int? HospitalId { get; set; }
        public Hospital Hospital { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int? SaleId { get; set; }
        public Sales Sale { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public int? AgentId { get; set; }
        public Agent Agent { get; set; }
        public string CommissionType { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
    }
}

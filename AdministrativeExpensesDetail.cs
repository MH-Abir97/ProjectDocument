using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("AdministrativeExpensesDetail", Schema = "Accounts")]
    public class AdministrativeExpensesDetail : BaseModel
    {
        public int Id { get; set; }
        public int AdministrativeExpensesId { get; set; }
        public AdministrativeExpense AdministrativeExpense { get; set; }
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int AccountLedgerId { get; set; }
        public virtual AccountLedger AccountLedger { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Particulars { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
    }
}

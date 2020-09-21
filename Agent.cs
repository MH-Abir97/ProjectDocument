using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Agent", Schema = "Accounts")]
    public class Agent : BaseModel
    {
        public int Id { get; set; }
        public string Area { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string CompanyName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string NID { get; set; }
        public string Photo { get; set; }
        public bool IsTerminated { get; set; }
        public bool IsSuspended { get; set; }
        public int? AccountLedgerGroupId { get; set; }
        public AccountLedgerGroup AccountLedgerGroup { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
        public string AdvanceBalance { get; set; }
        public string AdvanceBalanceRemark { get; set; }
        public bool BillByBill { get; set; }
        public bool OnAccount { get; set; }
        public bool HasCommission { get; set; }
        public double CommissionPercent { get; set; }
        public double CommissionAmount { get; set; }
    }
}

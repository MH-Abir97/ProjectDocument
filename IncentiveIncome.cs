using Pronali.Data.Models.Entity.POS;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("IncentiveIncome", Schema = "Accounts")]
    public class IncentiveIncome : BaseModel
    {
        public int Id { get; set; }
        public int? SaleId { get; set; }
        public Sales Sale { get; set; }
        public int? ChallanId { get; set; }
        public Challan Challan { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public string ServiceName { get; set; }
        public decimal BillAmount { get; set; }
        public double Quantity { get; set; }
        public double QuantityUnit { get; set; }
        public decimal Discount { get; set; }
        public decimal VAT { get; set; }
        public decimal Amount { get; set; }
    }
}

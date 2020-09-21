using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("CommissionIncomeDetail", Schema = "Accounts")]
    public class CommissionIncomeDetail : BaseModel
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public int? SaleId { get; set; }
        public Sales Sale { get; set; }
        public int? ChallanId { get; set; }
        public Challan Challan { get; set; }
        public DateTime BillingDate { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public string PaymentType { get; set; }
        public string ChequeNo { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public string Others { get; set; }
        public decimal AmountPaid { get; set; }
        public bool FullPaid { get; set; }
    }
}

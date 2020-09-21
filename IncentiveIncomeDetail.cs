using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("IncentiveIncomeDetail", Schema = "Accounts")]
    public class IncentiveIncomeDetail : BaseModel
    {
        public int Id { get; set; }
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
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BillAmount { get; set; }
        public string PaymentType { get; set; }
        public double QuantityUnit { get; set; }
        public decimal Discount { get; set; }
        public decimal AmountPaid { get; set; }
        public bool FullPaid { get; set; }
        public string Others { get; set; }
        public int? AccountTypeId { get; set; }
        public AccountType GetAccountType { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }
}

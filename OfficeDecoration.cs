using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("OfficeDecoration", Schema = "Accounts")]
    public class OfficeDecoration : BaseModel
    {
        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Particuler { get; set; }
        public string Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
        public int? ModelNo { get; set; }
        public string ProductType { get; set; }
        public string ProductFeature { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string ReturnPolicy { get; set; }
        public string ExchangeInfo { get; set; }
        public bool IsDepriciable { get; set; }
        public int? DepriciationId { get; set; }
        public Depriciation Depriciation { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
    }
}

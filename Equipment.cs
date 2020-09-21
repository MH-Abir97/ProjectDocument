using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Equipment", Schema = "Accounts")]
    public class Equipment : BaseModel
    {
        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Particuler { get; set; }
        public string Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
        public int? ModelNo { get; set; }
        public string ProductType { get; set; }
        public string ProductFeature { get; set; }
        public string GrantyCard { get; set; }
        public string WarrentyCard { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string ReturnPolicy { get; set; }
        public string ExchangeInfo { get; set; }
        public int? SerialNumber { get; set; }
        public string ImeiNo { get; set; }
        public int? DepriciationId { get; set; }
        public Depriciation Depriciations { get; set; }
        public string Brand { get; set; }
        public string Manufacturer { get; set; }
        public string Orgin { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
    }
}

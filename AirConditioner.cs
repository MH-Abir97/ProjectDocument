using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("AirConditioner", Schema = "Accounts")]
    public class AirConditioner : BaseModel
    {
        public int Id { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public int AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
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
        public string ModelNo { get; set; }
        public string AccountType { get; set; }
        public string ProductFeatures { get; set; }
        public string GrantyCard { get; set; }
        public string WarentyCard { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string ReturnPolicy { get; set; }
        public string ExchangeInfo { get; set; }
        public string Brand { get; set; }
        public string Depriciation { get; set; }
        public string Manufacturer { get; set; }
        public string Orgin { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
    }
}

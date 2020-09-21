using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("FinishedGood", Schema = "Accounts")]
    public class FinishedGood : BaseModel
    {
        public int Id { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public int? ProductSizeId { get; set; }
        public ProductSize ProductSize { get; set; }
        public int? ProductColorId { get; set; }
        public ProductColor ProductColor { get; set; }
        public int? StoreId { get; set; }
        public Store Store { get; set; }
        public int? PackageUnitId { get; set; }
        public PackageUnit PackageUnit { get; set; }
        public string Model { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DelearPrice { get; set; }
        public decimal? TradingPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public double? Quantity { get; set; }
        public decimal? CostPrice { get; set; }
        public string ImeiNo { get; set; }
        public string SerialNo { get; set; }
        public string BatchNo { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime MfgDate { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}

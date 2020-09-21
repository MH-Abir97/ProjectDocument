using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Inventory", Schema = "Accounts")]
    public class Inventory : BaseModel
    {
        public int Id { get; set; }
        public int? SaleId { get; set; }
        public Sales Sale { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public int? PackageUnitId { get; set; }
        public PackageUnit PackageUnit { get; set; }
        public string ModelNo { get; set; }
        public double? RemainingQuantity { get; set; }
        public double? SalesReturnQuantity { get; set; }
        public double? PurchaseQuantity { get; set; }
        public double? PurchaseReturnQuantity { get; set; }
        public double? SalesQuantity { get; set; }
        public int? UnitId { get; set; }
        public Unit Unit { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? TradinglPrice { get; set; }
        public decimal? DelearPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public DateTime? MfgDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string SerialNo { get; set; }
        public string BatchNo { get; set; }
        public string ImeiNo { get; set; }
        public int? ProductSizeId { get; set; }
        public ProductSize ProductSize { get; set; }
        public int? ProductColorId { get; set; }
        public ProductColor ProductColor { get; set; }
        public int? StoreId { get; set; }
        public Store Store { get; set; }
        public string ItemCode { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public string RackNo { get; set; }
        public int? HSCodeId { get; set; }
        public HSCode HSCode { get; set; }
    }
}

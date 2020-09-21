using Pronali.Data.Models.Entity.POS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Factory", Schema = "Accounts")]
    public class Factory : BaseModel
    {
        public int Id { get; set; }
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public int? AccountLedgerId { get; set; }
        public AccountLedger AccountLedger { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Particuler { get; set; }
        public string Description { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Note { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string ReturnPolicy { get; set; }
        public string ExchangeInfo { get; set; }
        public bool Depriciation { get; set; }
        public int? DepriciationId { get; set; }
        public virtual Depriciation Depriciations { get; set; }
        public string PloteNo { get; set; }
        public string PloteAddress { get; set; }
        public string PloteDagNo { get; set; }
        public string PloteKhotiyanNo { get; set; }
        public string PloteRegNo { get; set; }
        public string KhotiyanNo { get; set; }
        public string Dalil { get; set; }
        public string FlatNo { get; set; }
        public string Floor { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAddress { get; set; }
        public string SellerName { get; set; }
        public string SellerAddress { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
    }
}

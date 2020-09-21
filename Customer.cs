using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.POS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Customer", Schema = "Accounts")]
    public class Customer : BaseModel
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public CustomerType CustomerType { get; set; }
        [StringLength(50)]
        public string CompanyName { get; set; }
        [StringLength(50)]
        public string ContactPerson { get; set; }
        public string Mobile { get; set; }
        public string LandPhone { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Address { get; set; }
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public decimal Balance { get; set; }
        public string BalanceRemark { get; set; }
        public decimal PdcBalance { get; set; }
        public decimal PdcBalaceRemark { get; set; }
        public bool BillByBIll { get; set; }
        public bool OnAccount { get; set; }
        [Display(Name = "Special Price")]
        public int? PriceId { get; set; }
        public Price Price { get; set; }
        public bool IsAfilate { get; set; }
        public int? AccountLedgerGroupId { get; set; }
        public AccountLedgerGroup AccountLedgerGroup { get; set; }
        public bool HasCommission { get; set; }
    }
}

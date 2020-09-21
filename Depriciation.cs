using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("Depriciation", Schema = "Accounts")]
    public class Depriciation : BaseModel
    {
        public int Id { get; set; }
        public double? LifeTime { get; set; }
        public decimal? SalvageValue { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DepriciationRateInAmount { get; set; }
        public decimal? DepriciationRateInPercent { get; set; }
    }
}

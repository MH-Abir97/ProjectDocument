using Pronali.Data.Models.Entity.POS;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronali.Data.Models.Entity.Accounts
{
    [Table("HSCode", Schema = "Accounts")]
    public class HSCode : BaseModel
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public string HsCode { get; set; }
        public decimal VAT { get; set; }
        public string CustomsDuty { get; set; }
        public string ExciseDuty { get; set; }
        public string Surcharge { get; set; }
        public string TariffSchedule { get; set; }
    }
}

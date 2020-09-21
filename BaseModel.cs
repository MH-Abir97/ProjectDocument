using System;
using Pronali.Data.Models.Interfaces;

namespace Pronali.Data.Models
{
    public class BaseModel : IBaseModel
    {
        public string Mod { get; set; }
        public string PublicIp { get; set; }
        public string PrivateIp { get; set; } 
        public string MacAddress { get; set; }
        public string Vpn { get; set; } 
        public string CreatedBy { get; set; } 
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}

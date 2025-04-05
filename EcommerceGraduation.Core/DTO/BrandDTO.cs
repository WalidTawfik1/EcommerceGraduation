using EcommerceGraduation.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
    public record BrandDTO
     (string BrandCode, string BrandName);

        public record AddBrandDTO
        {
            [JsonIgnore]
            public string BrandCode { get; set; } = GenerateCode.GetCode();
            public string BrandName { get; init; }
            public AddBrandDTO(string BrandName)
            {
            this.BrandName = BrandName;
                BrandCode = GenerateCode.GetCode();
            }
        }
        public record UpdateBrandDTO
             (string BrandCode, string BrandName);
    
}

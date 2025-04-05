using EcommerceGraduation.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
    public record SubCategoryDTO
    (string SubCategoryCode, string CategoryCode, string Name);

    public record AddSubCategoryDTO
    {
        [JsonIgnore]
        public string SubCategoryCode { get; set; } = GenerateCode.GetCode();
        public string Name { get; init; }
        public string CategoryCode { get; set; }
        public AddSubCategoryDTO(string Name, string categoryCode)
        {
            this.Name = Name;
            SubCategoryCode = GenerateCode.GetCode();
            CategoryCode = categoryCode;
        }

    }
    public record UpdateSubCategoryDTO
         (string SubCategoryCode, string Name, string CategoryCode);
}

using EcommerceGraduation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
    using EcommerceGraduation.Core.Sharing;
    using System;
    using System.Linq;
    using System.Text.Json.Serialization;

    public record CategoryDTO
     (string CategoryCode, string Name);

    public record  AddCategoryDTO
    {
        [JsonIgnore]
        public string CategoryCode { get; set; } = GenerateCode.GetCode();
        public string Name { get; init; }
        public AddCategoryDTO(string name)
        {
            Name = name;
            CategoryCode = GenerateCode.GetCode();
        }
    }
   public record UpdateCategoryDTO
        (string CategoryCode, string Name);

}

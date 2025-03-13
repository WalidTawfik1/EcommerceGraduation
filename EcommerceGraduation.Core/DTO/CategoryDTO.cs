using EcommerceGraduation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.DTO
{
    using System;
    using System.Linq;


    public record CategoryDTO
     (string CategoryCode, string Name);

    public record  AddCategoryDTO
    {
        public string CategoryCode { get; set; } = GenerateCategoryCode();
        public string Name { get; init; }

        public static string GenerateCategoryCode()
        {
            Random random = new();
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string digits = "0123456789";

            return $"{letters[random.Next(letters.Length)]}" +  // Letter
                   $"{digits[random.Next(digits.Length)]}" +   // Digit
                   $"{digits[random.Next(digits.Length)]}" +   // Digit
                   $"{digits[random.Next(digits.Length)]}" +   // Digit
                   $"{letters[random.Next(letters.Length)]}" + // Letter
                   $"{digits[random.Next(digits.Length)]}";    // Digit
        }
    }
   public record UpdateCategoryDTO
        (string CategoryCode, string Name);

}

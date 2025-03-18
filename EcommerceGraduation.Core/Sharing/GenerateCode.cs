using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceGraduation.Core.Sharing
{
    public static class GenerateCode
    {
        private static readonly Random random = new();
        private static readonly string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string digits = "0123456789";

        public static string GetCode()
        {
            return $"{letters[random.Next(letters.Length)]}" +  // Letter
                   $"{digits[random.Next(digits.Length)]}" +   // Digit
                   $"{digits[random.Next(digits.Length)]}" +   // Digit
                   $"{digits[random.Next(digits.Length)]}" +   // Digit
                   $"{letters[random.Next(letters.Length)]}" + // Letter
                   $"{digits[random.Next(digits.Length)]}";    // Digit
        }

        public static string GetBarcode()
        {
            return random.Next(1000000, 9999999).ToString();
        }

        public static string GetCustomerId()
        {
            return random.Next(2000, 9999999).ToString();

        }
    }

}

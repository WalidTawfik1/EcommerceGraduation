namespace Ecom.Core.Entities
{
    public class BasketItem
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public String Category { get; set; }
        public String Photo { get; set; }
    }
}
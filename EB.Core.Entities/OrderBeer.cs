
namespace EB.Core.Entities
{
    public class OrderBeer
    {
        public int BeerID { get; set; }
        public Beer Beer { get; set; }
        public int OrderID { get; set; }
        public Order Order { get; set; }
        public int Amount { get; set; }
    }
}

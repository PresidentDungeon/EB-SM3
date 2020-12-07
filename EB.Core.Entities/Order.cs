using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.Entities
{
    public class Order
    {
        public int ID { get; set; }
        public DateTime OrderCreated { get; set; }
        public double AccumulatedPrice { get; set; }
        public Customer Customer { get; set; }
        public bool OrderFinished { get; set; }
        public List<OrderBeer> OrderBeers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.Entities
{
    public class Order
    {
        public int ID { get; set; }
        public DateTime OrderCreated { get; set; }
        public DateTime OrderSent { get; set; }
        public double AccumulatedPrice { get; set; }
        public Beer[] Beers { get; set; }
    }
}

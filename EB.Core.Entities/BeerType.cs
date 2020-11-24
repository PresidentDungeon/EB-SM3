using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.Entities
{
    public class BeerType
    {
        public int ID { get; set; }
        public string TypeName { get; set; }
        public Beer[] Beers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.Entities
{
    public class Beer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Percentage { get; set; }
        public double IBU { get; set; }
        public double EBC { get; set; }
        public int Stock { get; set; }
        public string ImageURL { get; set; }
        public BeerType Type  { get; set; }
        public Brand Brand { get; set; }
        //public Collection Collection { get; set; }
    }
}

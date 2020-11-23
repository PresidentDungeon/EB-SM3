using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.Entities
{
    public class Brand
    {
        public int ID { get; set; }
        public string BrandName { get; set; }
        // public string BrandDescription { get; set; }
        public Beer[] Beers { get; set; }
    }
}

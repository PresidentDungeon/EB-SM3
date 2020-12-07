using System.Collections.Generic;

namespace EB.Core.Entities
{
    public class Brand
    {
        public int ID { get; set; }
        public string BrandName { get; set; }
        public List<Beer> Beers { get; set; }
    }
}

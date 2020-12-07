using System.Collections.Generic;

namespace EB.Core.Entities
{
    public class BeerType
    {
        public int ID { get; set; }
        public string TypeName { get; set; }
        public List<Beer> Beers { get; set; }
    }
}

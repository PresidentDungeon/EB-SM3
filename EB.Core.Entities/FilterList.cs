using System.Collections.Generic;

namespace ProductShop.Core.Entities
{
    public class FilterList<T>
    {
        public int totalItems { get; set; }
        public List<T> List { get; set; }
    }
}

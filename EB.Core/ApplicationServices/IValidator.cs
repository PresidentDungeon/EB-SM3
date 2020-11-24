using EB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EB.Core.ApplicationServices
{
    public interface IValidator
    {
        public void ValidateBeer(Beer beer);
        public void ValidateType(BeerType type);
        public void ValidateBrand(Brand brand);
    }
}

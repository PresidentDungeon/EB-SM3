using EB.Core.DomainServices;
using EB.Core.Entities;
using ProductShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EB.Core.ApplicationServices.Impl
{
    public class BeerTypeService : IBeerTypeService
    {
        private IBeerTypeRepository TypeRepository;
        private IValidator Validator;

        public BeerTypeService(IBeerTypeRepository typeRepository, IValidator validator)
        {
            this.TypeRepository = typeRepository ?? throw new NullReferenceException("Repository can't be null");
            this.Validator = validator ?? throw new NullReferenceException("Validator can't be null");
        }

        public BeerType ValidateType(BeerType type)
        {
            this.Validator.ValidateType(type);
            return type;
        }

        public BeerType CreateType(BeerType type)
        {
            if (type != null)
            {
                this.Validator.ValidateType(type);
                return TypeRepository.AddType(type);
            }
            return null;
        }

        public List<BeerType> GetAllTypes()
        {
            return TypeRepository.ReadTypes().ToList();
        }

        public FilterList<BeerType> GetTypesFilterSearch(Filter filter)
        {
            if (filter.CurrentPage < 0 || filter.ItemsPrPage < 0)
            {
                throw new InvalidDataException("Page or items per page must be above zero");
            }

            return TypeRepository.ReadTypesFilterSearch(filter);
        }

        public BeerType GetTypeById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }

            return TypeRepository.ReadTypeById(id);
        }

        public BeerType UpdateType(BeerType type)
        {
            if (type == null)
            {
                throw new ArgumentException("Updating type does not exist");
            }
            Validator.ValidateType(type);
            if (GetTypeById(type.ID) == null)
            {
                throw new InvalidOperationException("No type with such ID found");
            }

            return TypeRepository.UpdateTypeInRepo(type);
        }

        public BeerType DeleteType(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Incorrect ID entered");
            }
            if (GetTypeById(id) == null)
            {
                throw new InvalidOperationException("No type with such ID found");
            }
            return TypeRepository.DeleteTypeInRepo(id);
        }
    }
}

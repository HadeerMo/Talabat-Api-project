using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.core.Entities;

namespace Talabat.core.Specifications
{
    public class ProductWithBrandAndTypeSpecification:BaseSpecification<Product>
    {
        public ProductWithBrandAndTypeSpecification(ProductSpecParams productParams) :base(p =>
                  (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search))&&
                  (!productParams.BrandId.HasValue || p.ProductBrandId == productParams.BrandId.Value) &&
                  (!productParams.TypeId.HasValue  || p.ProductTypeId  == productParams.TypeId.Value)
                  )
        {
            AddIncludes(p => p.ProductType);
            AddIncludes(p => p.ProductBrand);

            ApplyPagination(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);
            
            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    case "name":
                        AddOrderBy(p => p.Name);
                        break;
                    default:
                        AddOrderBy(p => p.Id);
                        break;
                }
            }

            
        }
        public ProductWithBrandAndTypeSpecification(int id):base(p=>p.Id == id)
        {
            AddIncludes(p => p.ProductType);
            AddIncludes(p => p.ProductBrand);
        }
    }
}

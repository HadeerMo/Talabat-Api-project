using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.core.Entities;
using Talabat.core.Entities.OrderAggregate;

namespace Talabat.repository.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context , ILoggerFactory loggerFactory)
        {
            try
            {
                if (!context.ProductBrands.Any())
                {
                    var brandsData = File.ReadAllText("../Talabat.repository/Data/DataSeed/brands.json");
                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                    foreach(var brand in brands)
                        context.Set<ProductBrand>().Add(brand);
                }
                if (!context.ProductTypes.Any())
                {
                    var typesData = File.ReadAllText("../Talabat.repository/Data/DataSeed/types.json");
                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                    foreach (var type in types)
                        context.Set<ProductType>().Add(type);
                }
                if (!context.Products.Any())
                {
                    var ProductsData = File.ReadAllText("../Talabat.repository/Data/DataSeed/products.json");
                    var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);
                    foreach (var product in Products)
                        context.Set<Product>().Add(product);
                }
                if (!context.DeliveryMethods.Any())
                {
                    var DeliveryMethodsData = File.ReadAllText("../Talabat.repository/Data/DataSeed/delivery.json");
                    var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);
                    foreach (var DeliveryMethod in DeliveryMethods)
                        context.Set<DeliveryMethod>().Add(DeliveryMethod);
                }
                await context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(ex, ex.Message);
            }
        }
    }
}

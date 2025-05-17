using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs.Conversions
{
    //to convert to DTOs
    public static class ProductConversion
    {

        //convert DTO to entity
        public static Product ToEntity(ProductDTO product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.Quantity,
            Price = product.Price
        };

        //convert entity to DTO
        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product, IEnumerable<Product>? products)
        {
            //return single
            if (product is not null || products is null)
            {
                var singleProduct = new ProductDTO(
                    product!.Id,
                    product.Name!,
                    product.Quantity,
                    product.Price
                    );
                return (singleProduct, null);
            }

            //return list
            if (product is null || products is not null)
            {
                var _products = products.Select(p =>
                                    new ProductDTO(p.Id, p.Name, p.Quantity, p.Price)).ToList();
                return (null, _products);
            }

            return (null, null);
        }
    }
}

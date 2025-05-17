using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
    public record ProductDTO
    (
        int Id,

        [Required]
        string Name,

        [Required, Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        int Quantity,

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        decimal Price
    );
}


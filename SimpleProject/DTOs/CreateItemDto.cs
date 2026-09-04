using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SimpleProject.DTOs;

public class CreateItemDto : IValidatableObject
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue)]
    public double Price { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    public IFormFile? Image { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Image != null)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(Image.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                yield return new ValidationResult(
                    "Only JPG, JPEG and PNG images are allowed.",
                    new[] { nameof(Image) }
                );
            }

            if (Image.Length > 5 * 1024 * 1024)
            {
                yield return new ValidationResult(
                    "Image size must not exceed 5 MB.",
                    new[] { nameof(Image) }
                );
            }
        }
    }
}
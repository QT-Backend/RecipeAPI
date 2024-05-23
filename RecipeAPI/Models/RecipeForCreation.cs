using System.ComponentModel.DataAnnotations;

namespace RecipeAPI.Models;

public class RecipeForCreation
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required]
    public List<string> Ingredients { get; set; } = new List<string>();
    [Required]
    public string Directions { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeAPI.Entities;

public class Recipe
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }
    [Required]
    public string CreatedBy { get; set; }

    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    [Required]
    public List<string> Ingredients { get; set; }
    [Required]
    public string Directions { get; set; }
}

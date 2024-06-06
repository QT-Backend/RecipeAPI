using System.ComponentModel.DataAnnotations;


namespace RecipeAPI.Models;

public class Admin
{
    [Required]
    public string user_id { get; set; } = string.Empty;
    [Required]
    public string email { get; set; } = string.Empty;
    [Required]
    public string picture { get; set; } = string.Empty;
    [Required]
    public string name { get; set; } = string.Empty;

}

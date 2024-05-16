using RecipeAPI.Entities;

namespace RecipeAPI.Services;

public interface IRecipeRepo
{
    Task<Recipe?> GetRecipeAsync(int id);
    Task<IEnumerable<Recipe>> GetAllRecipesAsync();
    Task<Recipe?> AddRecipeAsync(Recipe recipe);
    Task<Recipe?> UpdateRecipeAsync(int id, Recipe recipe);
    Task<Recipe?> DeleteRecipeAsync(int id);
}

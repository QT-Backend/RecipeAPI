﻿using RecipeAPI.Entities;

namespace RecipeAPI.Services;

public interface IRecipeRepo
{
    Task<Recipe?> GetRecipeAsync(int id);
    Task<IEnumerable<Recipe>> GetAllRecipesAsync();
    Task<IEnumerable<Recipe>> GetUserRecipesAsync(string email);
    Task<Recipe?> AddRecipeAsync(Recipe recipe);
    Task<Recipe?> UpdateRecipeAsync(int id, Recipe recipe, string role);
    Task<Recipe?> DeleteRecipeAsync(int id, string email, string role);
}

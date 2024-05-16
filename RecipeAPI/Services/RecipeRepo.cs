using Microsoft.EntityFrameworkCore;
using RecipeAPI.DbContexts;
using RecipeAPI.Entities;

namespace RecipeAPI.Services;

public class RecipeRepo : IRecipeRepo
{
    private readonly RecipeDBContext _recipeDBContext;

    public RecipeRepo(RecipeDBContext recipeDBContext)
    {
        _recipeDBContext = recipeDBContext;
    }
    public async Task<Recipe?> AddRecipeAsync(Recipe recipe)
    {
        try
        {
            if (recipe == null) 
            {
                return null;
            }
            var result = await _recipeDBContext.Recipes.AddAsync(recipe);
            await _recipeDBContext.SaveChangesAsync();
            return result.Entity;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Recipe?> DeleteRecipeAsync(int id)
    {
        try
        {
            var recipe = await _recipeDBContext.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return null;
            }
            _recipeDBContext.Remove(recipe);
            await _recipeDBContext.SaveChangesAsync();
            return recipe;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
    {
        try
        {
            var recipes =  await _recipeDBContext.Recipes.ToListAsync();
            return recipes;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<Recipe?> GetRecipeAsync(int id)
    {
        try
        {

            return await _recipeDBContext.Recipes.FindAsync(id);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<Recipe?> UpdateRecipeAsync(int id, Recipe recipe)
    {
        try
        {
            var myRecipe = await _recipeDBContext.Recipes.FindAsync(id);
            if (myRecipe == null)
            {
                return null;
            }
            myRecipe.Title = recipe.Title;
            myRecipe.Description = recipe.Description;
            myRecipe.Ingredients = recipe.Ingredients;
            myRecipe.Directions = recipe.Directions;
            await _recipeDBContext.SaveChangesAsync();
            return myRecipe;

        }
        catch (Exception)
        {

            throw;
        }
    }
}

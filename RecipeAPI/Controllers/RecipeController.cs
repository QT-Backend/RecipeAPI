using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.Entities;
using RecipeAPI.Services;

namespace RecipeAPI.Controllers
{
    [Route("[controller]s")]
    [ApiController]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeRepo _recipeRepo;

        public RecipeController(IRecipeRepo recipeRepo)
        {
            _recipeRepo = recipeRepo ?? throw new ArgumentNullException(nameof(recipeRepo));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            try
            {
                var recipe = await _recipeRepo.GetRecipeAsync(id);
                if (recipe == null)
                {
                    return NotFound();
                }
                return Ok(recipe);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                               "Error retrieving data from the database");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes()
        {
            try
            {
                var recipes = await _recipeRepo.GetAllRecipesAsync();
                return Ok(recipes);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                               "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> AddRecipe(Recipe recipe)
        {
            try
            {
                //TODO: sanitize recipe id with models
                var newRecipe = await _recipeRepo.AddRecipeAsync(recipe);
                if (newRecipe == null)
                {
                    return BadRequest();
                }
                return CreatedAtAction(nameof(GetRecipe), new { newRecipe.id }, newRecipe);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                               "Error adding data to the database");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Recipe>> UpdateRecipe(int id, Recipe recipe)
        {
            try
            {
                var updatedRecipe = await _recipeRepo.UpdateRecipeAsync(id, recipe);
                if (updatedRecipe == null)
                {
                    return NotFound();
                }
                return CreatedAtAction(nameof(GetRecipe), new { updatedRecipe.id }, updatedRecipe);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                               "Error updating the database");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Recipe>> DeleteRecipe(int id)
        {
            try
            {
                var deletedRecipe = await _recipeRepo.DeleteRecipeAsync(id);
                if (deletedRecipe == null)
                {
                    return NotFound();
                }
                return Ok(deletedRecipe);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                               "Error deleting recipe from the database");
            }
        }
    }
}

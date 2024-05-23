using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeAPI.Entities;
using RecipeAPI.Models;
using RecipeAPI.Services;
using System.Security.Claims;

namespace RecipeAPI.Controllers
{
    [Route("[controller]s")]
    [ApiController]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeRepo _recipeRepo;

        private readonly IConfiguration _config;
        public RecipeController(IRecipeRepo recipeRepo, IConfiguration config)
        {
            _recipeRepo = recipeRepo ?? throw new ArgumentNullException(nameof(recipeRepo));
            _config = config;
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

        [HttpGet]
        [Route("MyRecipes")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetUserRecipes()
        {
            try
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var recipes = await _recipeRepo.GetUserRecipesAsync(userEmail);
                return Ok(recipes);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                               "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> AddRecipe(RecipeForCreation recipe)
        {
            try
            {
                var myRecipe = new Recipe
                {
                    CreatedBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Ingredients = recipe.Ingredients,
                    Directions = recipe.Directions
                };
                    
                var newRecipe = await _recipeRepo.AddRecipeAsync(myRecipe);
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
        public async Task<ActionResult<Recipe>> UpdateRecipe(int id, RecipeForCreation recipe)
        {
            try
            {
                var myRecipe = new Recipe
                {
                    CreatedBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Ingredients = recipe.Ingredients,
                    Directions = recipe.Directions
                };
                var updatedRecipe = await _recipeRepo.UpdateRecipeAsync(id, myRecipe);
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
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var deletedRecipe = await _recipeRepo.DeleteRecipeAsync(id, userEmail, "notAdmin");
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

        [HttpDelete]
        [Route("admin/{id}")]
        public async Task<ActionResult<Recipe>> DeleteRecipeAdmin(int id)
        {
            try
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                if (userEmail != _config.GetValue<string>("Admins"))
                {
                    return BadRequest();
                }
                var deletedRecipe = await _recipeRepo.DeleteRecipeAsync(id, userEmail, "Admin");
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

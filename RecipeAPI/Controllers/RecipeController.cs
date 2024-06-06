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

                string? authHeader = HttpContext.Request.Headers["Authorization"];
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
                var updatedRecipe = await _recipeRepo.UpdateRecipeAsync(id, myRecipe, "notAdmin");
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

        [HttpPut("admin/{id}")]
        public async Task<ActionResult<Recipe>> UpdateRecipeAdmin(int id, RecipeForCreation recipe)
        {
            try
            {

                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

                if (!await isAdmin(userEmail))
                {
                    return BadRequest();
                }

                var myRecipe = new Recipe
                {
                    CreatedBy = userEmail,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Ingredients = recipe.Ingredients,
                    Directions = recipe.Directions
                };
                var updatedRecipe = await _recipeRepo.UpdateRecipeAsync(id, myRecipe, "Admin");
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
        //[Authorize("delete:recipe")]
        public async Task<ActionResult<Recipe>> DeleteRecipeAdmin(int id)
        {
            try
            {

                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                
                if (!await isAdmin(userEmail))
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

        private async Task<bool> isAdmin(string email)
        {

            try
            {
                string? token = _config.GetValue<string>("Token");
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://dev-etardgth2vfz55s5.us.auth0.com/api/v2/roles/rol_6YcDTKO2J2eFi7EE/users");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", "Bearer " + token);
                var response = await client.SendAsync(request);
                var admins = await response.Content.ReadFromJsonAsync<List<Admin>>();

                if (admins == null )
                {
                    return false;
                }

                foreach (var item in admins)
                {
                    if (item.email == email)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}

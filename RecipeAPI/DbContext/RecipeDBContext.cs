using RecipeAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace RecipeAPI.DbContexts;

public class RecipeDBContext : DbContext
{
    public RecipeDBContext(DbContextOptions<RecipeDBContext> options) : base(options)
    {

    }

    public DbSet<Recipe> Recipes { get; set; }
}

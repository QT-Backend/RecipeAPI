using Auth0.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RecipeAPI.DbContexts;
using RecipeAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    IConfigurationSection auth0Config = builder.Configuration.GetSection("Auth0");

    options.Domain = auth0Config["Domain"] ?? throw new Exception("Missing 'Domain' setting in Auth0 configuration");
    options.ClientId = auth0Config["ClientId"] ?? throw new Exception("Missing 'ClientId' setting in Auth0 configuration"); ;
    options.ClientSecret = auth0Config["ClientSecret"] ?? throw new Exception("Missing 'ClientSecret' setting in Auth0 configuration"); ;
    options.Scope = "openid profile email";
});

builder.Services.AddDbContextPool<RecipeDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RecipeConnection"))
);

builder.Services.AddScoped<IRecipeRepo, RecipeRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

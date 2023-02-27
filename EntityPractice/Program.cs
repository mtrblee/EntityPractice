using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

var factory = new CookbookContextFactory();
using var context = factory.CreateDbContext(args);

Console.WriteLine("Add Porridge for breakfast");

var porridge = new Dish { Title = "Breakfast Porridge", Notes = "Good stuff", Stars = 4 };
context.Dishes.Add(porridge);
await context.SaveChangesAsync();

Console.WriteLine($"Added Porridge (id = {porridge.Id}) successfully");
Console.WriteLine("Checking stars for Porridge");
var dishes = await context.Dishes
    .Where(d => d.Title.Contains("Porridge"))
    .ToListAsync();
if (dishes.Count != 1) Console.Error.WriteLine("Something bad happened");
Console.WriteLine($"Porridge has {dishes[0].Stars} stars");

Console.WriteLine("Change Porridge stars to 5");
porridge.Stars = 5;
await context.SaveChangesAsync();
Console.WriteLine("Change stars");
//Console.WriteLine("Removing Porridge from database");
//context.Dishes.Remove(porridge);
//await context.SaveChangesAsync();
//Console.WriteLine("Porridge Removed");

class Dish
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string? Notes { get; set; }
    public int? Stars { get; set; }
    public List<DishIngredient> Ingredient { get; set; } = new();
}

class DishIngredient
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string UnitOfMeasure { get; set; } = string.Empty;
    [Column(TypeName = "decimal(5, 2)")]
    public decimal Amount { get; set; }
    public Dish? Dish { get; set; }
    public int DishId { get; set; }
}

class CookbookContext : DbContext
{
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<DishIngredient> Ingredients { get; set; }

    public CookbookContext(DbContextOptions<CookbookContext> options)
        : base(options)
    { 
    }
}

class CookbookContextFactory : IDesignTimeDbContextFactory<CookbookContext>
{
    public CookbookContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var optionsBuilder = new DbContextOptionsBuilder<CookbookContext>();
        optionsBuilder
            .UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
        return new CookbookContext(optionsBuilder.Options);
    }
}
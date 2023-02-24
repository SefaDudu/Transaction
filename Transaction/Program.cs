// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage;//transaction

ApplicationDbContext context = new();

IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

try
{
    Person p = new() { Name = "Sefa" };
    await context.Persons.AddAsync(p);
    await context.SaveChangesAsync();
    await context.Orders.AddAsync(new Order()
    {
        Description = "",
        PersonId = p.Id,
    });
    await context.SaveChangesAsync();
    await transaction.CommitAsync();


}
catch (Exception)
{
    await transaction.RollbackAsync();
}

public class Person
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Order> Orders { get; set; }
}
public class Order
{
    [Key]
    public int Id { get; set; }
    public string Description { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; }
}
class ApplicationDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("host=localhost; port=5432; database=transaction; username=postgres; password=postgrespw");
    }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }
}
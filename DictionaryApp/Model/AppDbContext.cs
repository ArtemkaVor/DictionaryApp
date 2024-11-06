using DictionaryApp.Model;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=appdatabase.db");
        }
    }

    public async Task<List<Organization>> GetAllOrganizationsAsync()
    {
        return await Organizations.Include(o => o.Employees).ToListAsync(); //
    }

    public async Task AddOrganizationAsync(Organization organization)
    {
        await Organizations.AddAsync(organization);
        await SaveChangesAsync();
    }

    public async Task DeleteOrganizationAsync(Organization organization)
    {
        Organizations.Remove(organization);
        await SaveChangesAsync();
    }




}
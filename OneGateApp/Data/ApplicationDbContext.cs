using Microsoft.EntityFrameworkCore;

namespace NeoOrder.OneGate.Data;

public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Setting> Settings { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<DApp> DApps { get; set; }
}

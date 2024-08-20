using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WajeSmartAssessment.Domain;
using WajeSmartAssessment.Domain.Common;

namespace WajeSmartAssessment.Infrastructure;
public class ApplicationDbContext : IdentityDbContext
{
    private readonly IHttpContextAccessor? _contextAccessor;
    private readonly IConfiguration? _config;
    public ApplicationDbContext(IHttpContextAccessor contextAccessor, IConfiguration config)
    {
        _config = config;
        _contextAccessor = contextAccessor;
    }
    public ApplicationDbContext()
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _config?.GetConnectionString("DefaultConnection");

        optionsBuilder.UseSqlServer(connectionString, x => {
            x.EnableRetryOnFailure();
        });
        //Configure your DbContext here
        
        base.OnConfiguring(optionsBuilder);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();
        var currentTime = DateTime.Now;
        var currentUser = _contextAccessor?.HttpContext?.User?.Identity?.Name;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentUser ?? "System";
                entry.Entity.CreatedOn = currentTime;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedBy = currentUser ?? "System";
                entry.Entity.LastModifiedOn = currentTime;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Blog> Blogs { get; set; }
}

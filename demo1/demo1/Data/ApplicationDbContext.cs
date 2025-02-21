using demo1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<RolePermission>? RolePermissions { get; set; }
    public DbSet<Product>? Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Đảm bảo mỗi User-Role chỉ có 1 quyền duy nhất
        modelBuilder.Entity<RolePermission>()
            .HasIndex(rp => new { rp.UserId, rp.RoleId })
            .IsUnique();
    }
}
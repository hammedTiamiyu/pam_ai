using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PAMAi.Infrastructure.Identity.Models;

namespace PAMAi.Infrastructure.Identity;

public class IdentityContext: IdentityDbContext<User>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable(name: "Role", t => t.HasComment("Application roles."));
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable(name: "RoleClaim");
        });

        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable(name: "UserClaim");
        });

        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable(name: "UserLogin");
        });

        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable(name: "UserRole");
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable(name: "UserToken");
        });

        builder.Entity<User>(entity =>
        {
            entity.ToTable(name: "User", t => t.HasComment("Application users."));

            entity.OwnsMany(u => u.RefreshTokens, owned =>
            {
                owned.HasIndex(r => r.Token)
                    .IsUnique(unique: true);

                owned.Ignore(r => r.IsRevoked);

                owned.Ignore(r => r.IsActive);

                owned.Ignore(r => r.IsExpired);
            });
        });
    }
}

using ECSina.Db.Entities;
using ECSina.Db.Entities.Auth;
using ECSina.Db.Entities.Forums;
using Microsoft.EntityFrameworkCore;

namespace ECSina.Db;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataEntity>();
        modelBuilder.Entity<DataComponent>(builder =>
        {
            builder.UseTptMappingStrategy();
            builder.HasOne(x => x.Entity).WithMany(x => x.Components);
            builder.HasOne(x => x.CreatedBy).WithMany();
        });

        modelBuilder.Entity<HierarchyComponent>();
        modelBuilder.Entity<UserComponent>(builder =>
        {
            builder.HasIndex(x => x.NormalizedLogin).IsUnique();
        });
        modelBuilder.Entity<PasswordComponent>();
        modelBuilder.Entity<RolesComponent>();

        modelBuilder.Entity<Message>();
        modelBuilder.Entity<TopicComponent>();
        modelBuilder.Entity<ForumComponent>();

        base.OnModelCreating(modelBuilder);
    }
}

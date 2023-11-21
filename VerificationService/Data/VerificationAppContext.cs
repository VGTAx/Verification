using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VerificationApp.Models;

namespace VerificationApp.Data
{
  public class VerificationAppContext : DbContext
  {
    public VerificationAppContext(DbContextOptions<VerificationAppContext> options) : base(options) { }

    public DbSet<InspectingOrganization> InspectingOrganizations { get; set; } 
    public DbSet<SmallBusinessEntity> SmallBusinessEntities { get; set; }
    public DbSet<Verification> Verifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfiguration(new VerificationConfiguration());
      modelBuilder.ApplyConfiguration(new SmallBusinessEntityConfiguration());
      modelBuilder.ApplyConfiguration(new InspectingOrganizationConfiguration());
    }    
  }

  /// <summary>
  /// Configure <see cref="Verififcation"/>
  /// </summary>
  public class VerificationConfiguration : IEntityTypeConfiguration<Verification>
  {
    public void Configure(EntityTypeBuilder<Verification> builder)
    {
      builder.HasKey(x => x.Id);      
    }
  }

  /// <summary>
  /// Configure <see cref="SmallBusinessEntity"/>
  /// </summary>
  public class SmallBusinessEntityConfiguration : IEntityTypeConfiguration<SmallBusinessEntity>
  {
    public void Configure(EntityTypeBuilder<SmallBusinessEntity> builder)
    {
      builder.HasKey(x => x.Id);

      builder
        .HasIndex(s => s.Name)
        .IsUnique();

      builder.HasData(
        new SmallBusinessEntity { Id = Guid.NewGuid().ToString(), Name = "Amazon" },
        new SmallBusinessEntity { Id = Guid.NewGuid().ToString(), Name = "Microsoft" },
        new SmallBusinessEntity { Id = Guid.NewGuid().ToString(), Name = "Apple" },
        new SmallBusinessEntity { Id = Guid.NewGuid().ToString(), Name = "Meta" },
        new SmallBusinessEntity { Id = Guid.NewGuid().ToString(), Name = "Google" }
      );
    }
  }

  /// <summary>
  /// Configure <see cref="InspectingOrganization"/>
  /// </summary>
  public class InspectingOrganizationConfiguration : IEntityTypeConfiguration<InspectingOrganization>
  {
    public void Configure(EntityTypeBuilder<InspectingOrganization> builder)
    {
      builder.HasKey(x => x.Id);

      builder
        .HasIndex(x => x.Name)
        .IsUnique();

      builder.HasData(
        new InspectingOrganization { Id = Guid.NewGuid().ToString(), Name = "Роспотребнадзор" },
        new InspectingOrganization { Id = Guid.NewGuid().ToString(), Name = "Роскомнадзор" },
        new InspectingOrganization { Id = Guid.NewGuid().ToString(), Name = "Росстандарт" },
        new InspectingOrganization { Id = Guid.NewGuid().ToString(), Name = "Федеральная налоговая служба" },
        new InspectingOrganization { Id = Guid.NewGuid().ToString(), Name = "Росприроднадзор " }
      );
    }
  }
}

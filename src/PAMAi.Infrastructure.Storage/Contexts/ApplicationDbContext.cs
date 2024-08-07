namespace PAMAi.Infrastructure.Storage.Contexts;

public sealed class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Asset> Assets { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<TermsOfService> TermsOfService { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<UserTermsOfServiceConsent> UserTermsOfServiceConsents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

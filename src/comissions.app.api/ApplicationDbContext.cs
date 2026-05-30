using comissions.app.api.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace comissions.app.api;

public class ApplicationDbContext:DbContext
{
    private readonly ApplicationDatabaseConfigurationModel _configuration;
    
    public ApplicationDbContext(ApplicationDatabaseConfigurationModel configuration, DbContextOptions<ApplicationDbContext> options):base(options)
    {
        _configuration = configuration;
    }

    public ApplicationDbContext()
    {
        _configuration = null;
    }

    public ApplicationDbContext(ApplicationDatabaseConfigurationModel configuration)
    {
        _configuration = null;
    }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
        {
            Host = _configuration?.Host ?? "localhost",
            Port = _configuration?.Port ?? 5432,
            Database = _configuration?.Database ?? "comissionsapp",
            // No hardcoded credential fallbacks. At runtime credentials come from the
            // injected configuration model (which throws if unset). At design time
            // (parameterless ctor, _configuration == null) they come from environment
            // variables so tooling never depends on baked-in secrets.
            Username = _configuration?.Username
                       ?? Environment.GetEnvironmentVariable("DATABASE__USERNAME")
                       ?? throw new InvalidOperationException("Database username is not configured."),
            Password = _configuration?.Password
                       ?? Environment.GetEnvironmentVariable("DATABASE__PASSWORD")
                       ?? throw new InvalidOperationException("Database password is not configured.")
        };
        optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);
        base.OnConfiguring(optionsBuilder);
    }
    
    #region DB Sets

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<ArtistPageSettings> ArtistPageSettings { get; set; }= null!;
    public DbSet<UserArtist> UserArtists { get; set; }= null!;
    public DbSet<ArtistRequest> ArtistRequests { get; set; }= null!;
    public DbSet<ArtistPortfolioPiece> ArtistPortfolioPieces { get; set; }= null!;
    public DbSet<Request> Requests { get; set; }= null!;
    public DbSet<RequestReference> RequestReferences { get; set; }= null!;
    public DbSet<RequestAsset> RequestAssets { get; set; }= null!;
    public DbSet<ArtistRequestMessage> ArtistRequestMessages { get; set; }= null!;
    public DbSet<Ban> Bans { get; set; }= null!;
    public DbSet<Suspension> Suspensions { get; set; }= null!;
    #endregion
}
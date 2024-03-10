using comissions.app.database.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace comissions.app.database;

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
            Username = _configuration?.Username ?? "postgres",
            Password = _configuration?.Password ?? "postgres"
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
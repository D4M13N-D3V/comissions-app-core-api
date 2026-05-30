
namespace comissions.app.api;

public class ApplicationDatabaseConfigurationModel
{
    private readonly IConfiguration _configuration;
    
    public ApplicationDatabaseConfigurationModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    // Non-sensitive connection details may fall back to local-dev defaults.
    public string Host => _configuration?.GetValue<string>("Database:Host") ?? "localhost";
    public int Port => _configuration?.GetValue<int>("Database:Port") ?? 5432;
    public string Database => _configuration?.GetValue<string>("Database:Database") ?? "comissionsapp";

    // Credentials must be supplied explicitly — never fall back to a hardcoded value.
    public string Username => _configuration?.GetValue<string>("Database:username")
                              ?? throw new InvalidOperationException("Database:username is not configured.");
    public string Password => _configuration?.GetValue<string>("Database:password")
                              ?? throw new InvalidOperationException("Database:password is not configured.");
}
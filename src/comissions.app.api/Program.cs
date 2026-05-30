using System.Reflection;
using System.Security.Claims;
using comissions.app.api.Middleware;
using comissions.app.api.Middleware.Authentication;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using Auth0.AspNetCore.Authentication;
using comissions.app.api;
using comissions.app.api.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Novu;
using Novu.DTO;
using Novu.Extensions;
using Novu.Interfaces;
using Novu.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSingleton<IStorageService,LocalStorageServiceProvider>();
builder.Services.AddSingleton<IPaymentService,StripePaymentServiceProvider>();

builder.Services.AddHttpContextAccessor();

// Cap request body size globally to mitigate unbounded-upload memory/disk DoS (10 MB).
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
});
builder.Services.AddEndpointsApiExplorer();

// Basic per-client fixed-window rate limiting to blunt abuse / brute force.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var partitionKey = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                           ?? context.Connection.RemoteIpAddress?.ToString()
                           ?? "anonymous";
        return System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
            new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
});
builder.Services.AddSingleton<ApplicationDatabaseConfigurationModel>();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Documentation",
        Version = "v1.0",
        Description = ""
    });
    options.ResolveConflictingActions(x => x.First());
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        BearerFormat = "JWT",
        Flows = new OpenApiOAuthFlows
        {
            Implicit  = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri($"{builder.Configuration.GetValue<string>("Auth0:Domain")}oauth/token"),
                AuthorizationUrl = new Uri($"{builder.Configuration.GetValue<string>("Auth0:Domain")}authorize?audience={builder.Configuration.GetValue<string>("Auth0:Audience")}"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenId" },
                    { "email", "Email" },
                    { "profile", "Profile" },
                    { "read:user",  "Read your user information." },
                    { "write:user", "Update your user information." },
                    { "read:artist", "Read settings and information about your artist profile."},
                    { "write:artist", "Update settings, page design, and other things about your artist profile."},
                    { "read:request", "View existing requests and their artwork."},
                    { "write:request", "Create new requests."},
                }
            }
        }
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new[] { "openid", "email", "profile" }
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

});
builder.Services.RegisterNovuClients(builder.Configuration).AddTransient<NovuClient>();

builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new LoggingFilter()); // Register a custom logging filter
    })
    .AddJsonOptions(options=>
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        );

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = $"{builder.Configuration.GetValue<string>("Auth0:Domain")}";
    options.Audience = $"{builder.Configuration.GetValue<string>("Auth0:Audience")}";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    
    options.AddPolicy("read:user", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:user", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("write:user", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:user", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("read:artist", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:artist", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("write:artist", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:artist", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("read:request", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:request", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("write:request", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:request", builder.Configuration.GetValue<string>("Auth0:Domain"))));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

// CORS: allow only the explicitly configured origins (comma-separated in Cors:AllowedOrigins).
const string CorsPolicyName = "DefaultCorsPolicy";
var allowedOrigins = builder.Configuration.GetValue<string>("Cors:AllowedOrigins")?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

// Enforce HSTS outside development.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

var dbContext = app.Services.GetService<ApplicationDbContext>();
dbContext.Database.Migrate();
app.UseSwagger();
app.UseSwaggerUI(settings =>
{
    settings.OAuthClientId(builder.Configuration.GetValue<string>("Auth0:ClientId"));
    settings.OAuthClientSecret(builder.Configuration.GetValue<string>("Auth0:ClientSecret"));
    settings.OAuthUsePkce();
    settings.ConfigObject.OAuth2RedirectUrl = "https://core-api.development.comissions.app/swagger/oauth2-redirect.html";
});
var defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("index.html"); // replace 'yourf
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors(CorsPolicyName);
app.UseAuthentication();
app.UseMiddleware<UserMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();


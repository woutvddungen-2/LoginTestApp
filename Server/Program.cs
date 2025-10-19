using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server;
using Server.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------- JWT Secret --------------------
string jwtSecret = new JWTString().Secret;

// -------------------- Authentication --------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var env = builder.Environment;
    var loggerFactory = LoggerFactory.Create(logging =>
    {
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
    });
    var log = loggerFactory.CreateLogger("JWT");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue("jwt", out var jwt))
            {
                if (env.IsDevelopment())
                    log.LogInformation("✅ JWT cookie received: {Snippet}", jwt[..Math.Min(20, jwt.Length)]);
                context.Token = jwt;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (env.IsDevelopment())
                log.LogWarning(context.Exception, "❌ JWT Authentication failed");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            if (env.IsDevelopment())
            {
                var claims = context.Principal?.Claims?.Select(c => $"{c.Type}: {c.Value}");
                if (claims != null)
                {
                    foreach (var c in claims)
                        log.LogInformation("🔹 Claim: {Claim}", c);
                }
            }
            return Task.CompletedTask;
        }
    };
}); ;
});

// -------------------- Authorization --------------------
builder.Services.AddAuthorization();

// -------------------- Controllers --------------------
builder.Services.AddControllers();

// -------------------- Swagger --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// -------------------- CORS --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm", policy =>
    {
        policy.WithOrigins("https://localhost:5002")
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// -------------------- Database --------------------
DbConnect.AddDatabase(builder.Services, builder.Configuration.GetSection("DatabaseSettings"));

var app = builder.Build();

// -------------------- Logging --------------------
var logger = app.Services.GetRequiredService<ILogger<Program>>();

if (DbConnect.TestDatabaseConnection(app.Services, out Exception? ex))
{
    logger.LogInformation("Successfully connected to the database.");
}
else
{
    logger.LogError(ex, "Failed to connect to the database.");
}

// -------------------- Middleware --------------------
app.UseCors("AllowBlazorWasm");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        c.RoutePrefix = "";
    });
    Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
}

app.MapControllers();
app.Run();


// -------------------- JWT Secret Helper --------------------
class JWTString
{
    public string Secret { get; private set; } = "ThisIsASuperSecretKey12345678901";
}

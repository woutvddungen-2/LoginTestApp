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
            string? token = null;
            if (context.Request.Cookies.TryGetValue("jwt", out var jwtCookie))
            {
                token = jwtCookie;
            }

            // Assign token to the context so middleware can validate it
            context.Token = token;

            if (!string.IsNullOrEmpty(token) && env.IsDevelopment())
            {
                log.LogInformation("JWT cookie received: {Snippet}...", token[..Math.Min(token.Length, 20)]);
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (env.IsDevelopment())
                log.LogWarning(context.Exception, "JWT Authentication failed");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            if (env.IsDevelopment() && context.Principal != null && context.Principal.Claims != null)
            {
                var logText = string.Join("; ",
                context.Principal.Claims
                    .Where(c => c.Type != "nbf" && c.Type != "iat")
                    .Select(c =>
                    {
                        if (c.Type == "exp" && long.TryParse(c.Value, out var expSeconds))
                        {
                            var expDate = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
                            return $"exp: {expDate:yyyy-MM-dd HH:mm:ss} UTC";
                        }
                        return $"{c.Type}: {c.Value}";
                    })
                );

                if (!string.IsNullOrWhiteSpace(logText))
                {
                    log.LogInformation("JWT Contains: {Snippet}", logText);
                }
            }            
            return Task.CompletedTask;
        }
    };
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

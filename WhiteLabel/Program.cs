using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WhiteLabel.Domain.Dto;
using WhiteLabel.Domain.Interfaces;
using WhiteLabel.Infra.Contexts;
using WhiteLabel.Infra.Repository;
using WhiteLabel.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "White Label API",
        Version = "v1",
        Description = "API for clinic management system - AMPARA",
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "support@whitelabel.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddSingleton<IContext, Context>();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError("Authentication failed: {Message}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token validated for user: {User}",
                context.Principal?.Identity?.Name ?? "Unknown");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.AddScoped<IRepositoryUser, RepositoryUser>();
builder.Services.AddScoped<IPatientRepository, RepositoryPatient>();
builder.Services.AddScoped<IRepositoryEvolution, RepositoryEvolution>();
builder.Services.AddScoped<IRepositoryMedicalReport, RepositoryMedicalReport>();

builder.Services.AddScoped<IServiceAuth, ServiceAuth>();
builder.Services.AddScoped<IServicePatient, ServicePatient>();
builder.Services.AddScoped<IServiceEvolution, ServiceEvolution>();
builder.Services.AddScoped<IServiceMedicalReport, ServiceMedicalReport>();
builder.Services.AddScoped<IServiceUser, ServiceUser>();

builder.Services.AddHealthChecks()
    .AddCheck("mongodb", () =>
    {
        var context = builder.Services.BuildServiceProvider().GetRequiredService<IContext>();
        return context.IsConnected()
            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("MongoDB is connected")
            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("MongoDB is not connected");
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IContext>();
    var startupLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    if (context.IsConnected())
    {
        startupLogger.LogInformation("✅ MongoDB connection verified successfully");
    }
    else
    {
        startupLogger.LogWarning("⚠️  MongoDB connection could not be verified");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "White Label API V1");
        c.RoutePrefix = "swagger";
    });
}

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🚀 White Label API starting...");
logger.LogInformation("📍 Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("⚙️  MongoDB: {ConnectionString}",
    builder.Configuration.GetSection("MongoDBSettings:ConnectionString").Value);

app.Run();
using Microsoft.OpenApi.Models;
using NoteApp.API.Middleware;
using NoteApp.Application.DependencyInjection;
using NoteApp.Infrastructure.DependencyInjection;
using NoteApp.Persistence.Contexts;
using NoteApp.Persistence.DependencyInjection;
using NoteApp.Persistence.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();

// Register Application & Persistence & Infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger JWT Authentication desteği
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "NoteApp API", 
        Version = "v1",
        Description = "JWT Authentication ve Role-based Authorization ile güvenli bir API."
    });
    
    // JWT Authentication için güvenlik tanımı
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Örnek: 'Bearer {token}'",
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// CORS Politikası ekle (gerekirse)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// SEED DATABASE
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<NoteAppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Veritabanı seed işlemi başlatılıyor...");
        await SeedData.InitializeAsync(context);
        logger.LogInformation("Veritabanı seed işlemi tamamlandı.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Seed işlemi sırasında bir hata oluştu: {ex.Message}");
    Console.WriteLine($"StackTrace: {ex.StackTrace}");
    // Uygulama başlangıcını engellemeden hata loglanır
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NoteApp API v1");
        c.RoutePrefix = "swagger";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.DefaultModelsExpandDepth(-1); // Models bölümünü gizle
    });
}

// Configure middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// Cors middleware
app.UseCors("AllowAll");

// Authentication ve Authorization middleware'lerini etkinleştir
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
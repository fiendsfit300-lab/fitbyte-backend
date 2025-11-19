using Gym_FitByte.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ===============================
//   CONFIGURACIÓN GENERAL
// ===============================

// Cargar variables de entorno (Render / Railway)
builder.Configuration.AddEnvironmentVariables();

// Obtener cadena de conexión desde Render o desde appsettings.json
var connectionString =
    builder.Configuration.GetConnectionString("MySqlConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");


// ===============================
//   BASE DE DATOS (MySQL)
// ===============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 39)))
);


// ===============================
//   SERIALIZACIÓN JSON (IMPORTANTE)
//   ✔ Evita ciclos infinitos Rutina → Ejercicios → Rutina
//   ✔ SOLUCIONA EL ERROR 500
// ===============================
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

// ===============================
//   CORS
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPolitica", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// ===============================
//   SWAGGER
// ===============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ===============================
//   BUILD APP
// ===============================
var app = builder.Build();


// ===============================
//   AUTO-MIGRACIONES
// ===============================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        db.Database.Migrate();
        Console.WriteLine("✔ Migraciones aplicadas correctamente en producción.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error al aplicar migraciones: " + ex.Message);
    }
}


// ===============================
//   SWAGGER (DEV + PROD)
// ===============================
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ===============================
//   MIDDLEWARE
// ===============================
app.UseHttpsRedirection();
app.UseCors("NuevaPolitica");
app.UseAuthorization();


// ===============================
//   ENDPOINTS
// ===============================
app.MapControllers();


// ===============================
//   RUN
// ===============================
app.Run();

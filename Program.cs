using Gym_FitByte.Data;
using Microsoft.EntityFrameworkCore;

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
//   CONTROLADORES + SWAGGER
// ===============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ===============================
//   BUILD APP
// ===============================
var app = builder.Build();


// ===============================
//   AUTO-MIGRACIONES
// ===============================
// Esto ejecuta "dotnet ef database update" automáticamente
// cada vez que Render levanta el contenedor.
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

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SieveApp.Data;
using SieveApp.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер
builder.Services.AddControllers();

// Настройка подключения к базе данных SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// Регистрация сервисов
builder.Services.AddScoped<SieveService>();
builder.Services.AddScoped<JwtService>(_ => new JwtService("your_secret_key")); // Замените "your_secret_key" на реальный секретный ключ

// Настройка аутентификации JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // В данной реализации не проверяем издателя токена
            ValidateAudience = false, // Не проверяем аудиторию
            ValidateLifetime = true, // Проверяем срок действия токена
            ValidateIssuerSigningKey = true, // Проверяем подпись токена
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key")) // Используем секретный ключ
        };
    });

// Настройка авторизации
builder.Services.AddAuthorization();

// Создание приложения
var app = builder.Build();

// Настройка middleware
app.UseAuthentication(); // Включение аутентификации
app.UseAuthorization(); // Включение авторизации

// Маршрутизация контроллеров
app.MapControllers();

// Инициализация базы данных при первом запуске
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated(); // Автоматически создает базу данных, если её нет
}

// Запуск приложения
app.Run();
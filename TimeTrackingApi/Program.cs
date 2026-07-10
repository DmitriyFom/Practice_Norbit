using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.Data;
using TimeTrackingApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Подключение к БД
builder.Services.AddDbContext<TimeTrackingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TimeTrackingDb")));

// Регистрация сервисов 
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITimeEntryService, TimeEntryService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Контроллеры 
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Swagger с XML-документацией
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Time Tracking API",
        Version = "v1",
        Description = "API для учета рабочего времени сотрудников компании",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Dmitriy Fominykh",
            Email = "dmitriy@example.com"
        }
    });

    // Подключаем XML-комментарии из кода
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// CORS (для Vue)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Swagger 
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Time Tracking API v1");
    c.RoutePrefix = string.Empty; 
});

app.UseHttpsRedirection();
app.UseCors("AllowVue");
app.UseAuthorization();
app.MapControllers();

app.Run();
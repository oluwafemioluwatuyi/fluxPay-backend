using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using fluxPay.Clients;
using fluxPay.Data;
using fluxPay.Interfaces.Repositories;
using fluxPay.Interfaces.Services;
using fluxPay.Repositories;
using fluxPay.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using FluxPay.Utils;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();


// Access configuration
var fineractConfig = builder.Configuration.GetSection("Fineract");

// Register TenantId and FineractClient properly
builder.Services.AddSingleton(fineractConfig["TenantId"]);

// Register FineractClient with HttpClient
builder.Services.AddHttpClient<FineractClient>();


builder.Services.AddScoped<IFineractApiService, FineractApiService>();
builder.Services.AddScoped<IOtpService, OtpService1>();

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<ITempUserRepository, TempUserRepository>();
builder.Services.AddScoped<OtpService>();


builder.Services.AddScoped<IEmailService>(provider =>
{
   var logger = provider.GetRequiredService<ILogger<EmailService>>();
    var environment = provider.GetRequiredService<IWebHostEnvironment>();
    var fineractApiService = provider.GetRequiredService<IFineractApiService>();
    var templatesFolderPath = Path.Combine(environment.ContentRootPath, "Emails");
    var clientService = provider.GetRequiredService<IClientService>();

    return new EmailService(templatesFolderPath, logger, builder.Configuration, fineractApiService, clientService);
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Make JwtSettings globally available
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// Retrieve the connection string from appsettings.json
string connectionString = builder.Configuration.GetConnectionString("FineractDatabase");

// Register the ClientService with the connection string
builder.Services.AddScoped<IClientService>(provider => new ClientService(connectionString));

// Bind SMTP settings from appsettings.json to a strongly-typed object
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));

builder.Services.AddDbContext<fluxPayDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerDatabase"));
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
// .AddJsonOptions(options =>
// {
//     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
//     // Uncomment the next line if you have a custom DateTime converter
//     // options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
// })
// .AddNewtonsoftJson(options =>
// {
//     options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter { AllowIntegerValues = true });
//     // Uncomment the next line if you have a custom DateTime converter
//     // options.SerializerSettings.Converters.Add(new MultiFormatDateTimeConverter());
// });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();  // Ensure routing is enabled
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();  // This is required to map your controller routes
});


app.Run();

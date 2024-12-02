using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using fluxPay.Clients;
using fluxPay.Interfaces.Services;
using fluxPay.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpClient<FineractClient>();

builder.Services.AddScoped<IFineractApiService, FineractApiService>();
builder.Services.AddScoped<AuthService>();

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

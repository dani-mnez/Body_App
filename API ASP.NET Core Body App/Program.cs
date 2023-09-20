using API_ASP.NET_Core_Body_App.Repositories;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var mongoDbSettings = configuration.GetSection("MongoDB");
var connectionString = mongoDbSettings["ConnectionString"];
var dbName = mongoDbSettings["DatabaseName"];
IMongoClient mongoDbClient = new MongoClient(connectionString);

var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
ConventionRegistry.Register("Camel case conversion", conventionPack, t => true);

builder.Services.AddScoped<IUserRepository>(sp => new UserRepository(mongoDbClient, dbName!));
builder.Services.AddScoped<IHistoricalDataRepository>(sp => new HistoricalDataRepository(mongoDbClient, dbName!));
builder.Services.AddScoped<IMealRepository>(sp => new MealRepository(mongoDbClient, dbName!));
builder.Services.AddScoped<IPhysicalDataRepository>(sp => new PhysicalDataRepository(mongoDbClient, dbName!));
builder.Services.AddScoped<INutritionalDataRepository>(sp => new NutritionalDataRepository(mongoDbClient, dbName!));
builder.Services.AddScoped<IFoodDataRepository>(sp => new FoodDataRepository(mongoDbClient, dbName!));


builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    // Con esta configuración hacemos que ignore la serialización global - esto se hace para que System.Text.Json gestione la serialización de los objetos
    // así se puede usar únicamente Newtonsoft para la lectura de la clase JsonPatchDocument
    options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        IgnoreSerializableInterface = true,
        IgnoreSerializableAttribute = true
    };
});


var app = builder.Build();

app.UseRouting();
app.MapControllers();

// Configurar el middleware para el entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();

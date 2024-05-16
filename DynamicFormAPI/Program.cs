using Microsoft.Azure.Cosmos;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = builder.Configuration;

// Add CosmosClient and configure logging
builder.Services.AddSingleton((provider) =>
{
    var endpointUri = config["ConnectionStrings:URI"];
    var primaryKey = config["ConnectionStrings:PrimaryKey"];
    var databaseName = config["ConnectionStrings:DatabaseName"];

    var cosmosClientOptions = new CosmosClientOptions
    {
        ApplicationName = databaseName,
        ConnectionMode = ConnectionMode.Direct,

        ServerCertificateCustomValidationCallback = (request, certificate, chain) =>
        {
            return true;
        }
    };

    var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    return new CosmosClient(endpointUri, primaryKey, cosmosClientOptions);
});

builder.Services.AddControllers();

// Add wildcard CORS policy
builder.Services.AddCors(o => o.AddPolicy("default", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Remove server name from response header
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("../swagger/v1/swagger.json", "Dynamic Form App v1"));

app.UseHttpsRedirection();
app.UseCors("default");

app.UseAuthorization();

app.MapControllers();

app.Run();

using DynamicFormAPI.Data;
using DynamicFormAPI.Dtos;
using DynamicFormAPI.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Net;
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

// Inject data seeding service
builder.Services.AddSingleton((serviceProvider) => {
    var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();    
    return new CosmosDbSeeder(cosmosClient, config);
});

// Set controller behaviour
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errorMessages = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .Distinct();

        var errorMessageString = string.Join("; ", errorMessages);
        var errorResponse = new ErrorResponseDTO(
                       HttpStatusCode.BadRequest,
                       errorMessageString
                   );

        return new BadRequestObjectResult(errorResponse);
    };
});

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

// Inject services
builder.Services.AddScoped<IFormService, FormService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();

// Remove server name from response header
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var app = builder.Build();

// Request and response logging middleware
app.UseMiddleware<RequestResponseLogging>();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("../swagger/v1/swagger.json", "Dynamic Form App v1"));

// Seed data on startup
var seeder = app.Services.GetRequiredService<CosmosDbSeeder>();
await seeder.SeedDataAsync();

app.UseHttpsRedirection();
app.UseCors("default");

app.UseAuthorization();

app.MapControllers();

app.Run();

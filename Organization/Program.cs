using Core.Secrets;
using MongoDB.Driver;
using Organization;
using Organization.Services;

var builder = WebApplication.CreateBuilder(args);

Initialization initialization = new Initialization(builder);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<MongoClient>(sp => new MongoClient(initialization.GetMongoDbUri()));
builder.Services.AddSingleton<ISecretsManager, BitwardenSecretsManager>();
builder.Services.AddSingleton<IOrganizationRepository, OrganizationRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
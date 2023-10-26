using System.Security.Claims;
using Core.Secrets;
using Cuplan.Organization.Models;
using Cuplan.Organization.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Organization;
using Organization.Config;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityProvider:Authority"];
    options.Audience = builder.Configuration["IdentityProvider:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

Initialization initialization = new(builder);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Other dependencies
builder.Services.AddSingleton<MongoClient>(sp => new MongoClient(initialization.GetMongoDbUri()));
builder.Services.AddSingleton<ISecretsManager, BitwardenSecretsManager>();
builder.Services.AddSingleton<ConfigurationReader>();

// Services
builder.Services.AddSingleton<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddSingleton<IMemberRepository, MemberRepository>();

// Models
builder.Services.AddSingleton<MemberManager>();
builder.Services.AddSingleton<OrganizationManager>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
using TechChallenge.Api.Configurations;
using TechChallenge.Application;
using TechChallenge.Data;
using TechChallenge.Data.Seeds;
using TechChallenge.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddSqlContext(builder.Configuration);
builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.AddSeeds();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
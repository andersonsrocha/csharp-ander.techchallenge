using TechChallengeUsers.Api.Configurations;
using TechChallengeUsers.Api.Middlewares;
using TechChallengeUsers.Application;
using TechChallengeUsers.Data;
using TechChallengeUsers.Data.Seeds;
using TechChallengeUsers.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddSqlContext(builder.Configuration);
builder.Services.AddSecurity(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();

builder.AddSerilog();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestMiddleware>();

app.AddSeeds();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
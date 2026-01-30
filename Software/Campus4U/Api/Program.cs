using Api.DI;
using Api.Middleware;
using Server.Application.Email;
using Server.Data.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEmail(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuth0(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ValidationMiddleware>();
app.MapControllers();

app.Run();

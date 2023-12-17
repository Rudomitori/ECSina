using ECSina.App.Setup;
using ECSina.App.Setup.Auth;
using ECSina.App.Setup.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Personal.json", true);

builder.SetupControllers();
builder.SetupSwagger();
builder.SetupLogging();
builder.SetupDb();
builder.SetupAuth();
builder.SetupExceptionHandling();
builder.SetupCore();

var app = builder.Build();

await app.ApplyMigrations();
app.UseHealthCheckSetup();
app.UseLoggingSetup();
app.UseExceptionHandlingSetup();
app.UseAuthSetup();
app.UseSwaggerSetup();
app.UseControllersSetup();

app.Run();

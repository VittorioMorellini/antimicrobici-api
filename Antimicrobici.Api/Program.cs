using Serilog.Events;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Antimicrobici.Core.Models;
using Antimicrobici.Core.Services;
using Antimicrobici.Core.Config;

var builder = WebApplication.CreateBuilder(args);
Microsoft.Extensions.Configuration.IConfiguration configuration = builder.Configuration;

var logger = new LoggerConfiguration()
               .MinimumLevel.Information()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
               .Enrich.FromLogContext()
               .WriteTo.RollingFile("logs/{Date}.txt")
               .CreateLogger();

builder.Logging.AddSerilog(logger);
builder.Services.AddDbContext<AmDbContext>(
    builder => builder.UseSqlServer(configuration.GetConnectionString("Antimicrobici")),
    // è necessario che sia Transient poichè ho necessità di compiere azioni asincrone
    // ogni Service così ha la propria versione unica del DbContext (con la propria connessione)
    ServiceLifetime.Transient
);
builder.Services.AddControllers()
    .AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

//Add the services to inject the dependency Injections
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
//Save options to save
builder.Services.Configure<ConfigOptions>(configuration.GetSection(ConfigOptions.OptionsName));
builder.Services.AddScoped<IPrincipalService , PrincipalService>();
builder.Services.AddScoped<IDataHelperService, DataHelperService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IRichiedenteService, RichiedenteService>();
builder.Services.AddScoped<IMaterialiService, MaterialiService>();
builder.Services.AddScoped<IRichiedenteService, RichiedenteService>();
builder.Services.AddScoped<IRichiestaService, RichiestaService>();
builder.Services.AddScoped<ITipoMaterialeService, TipoMaterialeService>();
builder.Services.AddScoped<ICentriDiCostoService, CentriDiCostoService>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<IMatScadutoService, MatScadutoService>();

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
app.UseRouting();
app.UseCors();
//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();
app.Run();
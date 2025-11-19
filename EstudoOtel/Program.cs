using EstudoOtel.Contexts;
using EstudoOtel.Telemetry;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        // Define o nome da aplicação, para que não se misture com métricas de outras aplicações
        resource.AddService(serviceName: "EstudoOtel", serviceVersion: "1.0.0");
    })
    .WithMetrics(options =>
    {
        options

        // Vincula as métricas padrão do ASP.NET 
        // Isso adicoina CPU, Memória, GC
        .AddAspNetCoreInstrumentation()
        
        // Métricas personalizadas
        .AddMeter(PessoaMetrica.METRICA_PESSOA)
        
        .AddOtlpExporter(collector =>
        {
            // AQUI ESTÁ A MÁGICA
            collector.Endpoint = new Uri(builder.Configuration["OtelCollector:Url"]!);
            collector.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        });
    });

builder.Services.AddSingleton<PessoaMetrica>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

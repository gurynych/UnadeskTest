using Microsoft.EntityFrameworkCore;
using UnadeskTest.Shared.Data;
using UnadeskTest.Shared.Messaging;
using UnadeskTest.Shared.Options;
using UnadeskTest.Worker.Pdf;

namespace UnadeskTest.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton<IRabbitMqConnectionProvider, RabbitMqConnectionProvider>();
            builder.Services.AddSingleton<IPdfTextExtractor, PdfPigTextExtractor>();
            builder.Services.AddScoped<IDocumentProcessingService, DocumentProcessingService>();

            builder.Services.AddHostedService<PdfProcessingWorker>();

            IHost host = builder.Build();
            await host.RunAsync();
        }
    }
}

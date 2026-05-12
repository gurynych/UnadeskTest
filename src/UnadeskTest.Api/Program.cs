using Microsoft.EntityFrameworkCore;
using UnadeskTest.Api.Infrastructure;
using UnadeskTest.Api.Infrastructure.Files;
using UnadeskTest.Api.Options;
using UnadeskTest.Api.Services;
using UnadeskTest.Api.Workers;
using UnadeskTest.Shared.Data;
using UnadeskTest.Shared.Messaging;
using UnadeskTest.Shared.Options;

namespace UnadeskTest.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection(FileStorageOptions.SectionName));
            builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection(OutboxOptions.SectionName));
            builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
            builder.Services.AddSingleton<IRabbitMqConnectionProvider, RabbitMqConnectionProvider>();
            builder.Services.AddScoped<IDocumentQueuePublisher, RabbitMqDocumentQueuePublisher>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();
            builder.Services.AddScoped<IOutboxService, OutboxService>();

            builder.Services.AddHostedService<OutboxPublisherWorker>();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.EnsureCreatedAsync();
            }

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();
            app.MapHealthChecks("/health");

            await app.RunAsync();
        }
    }
}

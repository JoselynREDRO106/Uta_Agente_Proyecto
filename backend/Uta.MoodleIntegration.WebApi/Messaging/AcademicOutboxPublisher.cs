using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Uta.MoodleIntegration.WebApi.Data;

namespace Uta.MoodleIntegration.WebApi.Messaging;

public sealed class AcademicOutboxPublisher(
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitMqSettings> settings,
    ILogger<AcademicOutboxPublisher> logger) : BackgroundService
{
    private static readonly TimeSpan IdleDelay = TimeSpan.FromSeconds(2);
    private readonly RabbitMqSettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost,
            AutomaticRecoveryEnabled = true
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            OutboxMessage? pending = null;
            try
            {
                pending = await GetNextMessageAsync(stoppingToken);
                if (pending is null)
                {
                    await Task.Delay(IdleDelay, stoppingToken);
                    continue;
                }

                await using var connection = await factory.CreateConnectionAsync(stoppingToken);
                await using var channel = await connection.CreateChannelAsync(
                    new CreateChannelOptions(
                        publisherConfirmationsEnabled: true,
                        publisherConfirmationTrackingEnabled: true),
                    cancellationToken: stoppingToken);
                await channel.ExchangeDeclareAsync(
                    _settings.Exchange,
                    ExchangeType.Topic,
                    durable: true,
                    cancellationToken: stoppingToken);
                await channel.QueueDeclareAsync(
                    _settings.Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: stoppingToken);
                await channel.QueueBindAsync(
                    _settings.Queue,
                    _settings.Exchange,
                    "academic.course.updated.*",
                    cancellationToken: stoppingToken);
                var body = Encoding.UTF8.GetBytes(pending.PayloadJson);
                await channel.BasicPublishAsync(
                    _settings.Exchange,
                    pending.EventType,
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true },
                    body,
                    stoppingToken);
                await MarkPublishedAsync(pending.Id, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "No se pudo publicar un evento del outbox académico; se reintentará.");
                if (pending is not null)
                {
                    await RecordAttemptAsync(pending.Id, exception.Message, stoppingToken);
                }

                await Task.Delay(IdleDelay, stoppingToken);
            }
        }
    }

    private async Task<OutboxMessage?> GetNextMessageAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MoodleIntegrationDbContext>();
        return await dbContext.Outbox.AsNoTracking()
            .Where(item => item.PublishedAtUtc == null && item.NextAttemptAtUtc <= DateTime.UtcNow)
            .OrderBy(item => item.CreatedAtUtc)
            .Select(item => new OutboxMessage(item.Id, item.EventType, item.PayloadJson))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task MarkPublishedAsync(Guid eventId, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MoodleIntegrationDbContext>();
        var item = await dbContext.Outbox.SingleAsync(
            row => row.Id == eventId, cancellationToken);
        item.PublishedAtUtc = DateTime.UtcNow;
        item.LastError = null;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task RecordAttemptAsync(
        Guid eventId,
        string error,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MoodleIntegrationDbContext>();
        var item = await dbContext.Outbox.SingleOrDefaultAsync(
            row => row.Id == eventId && row.PublishedAtUtc == null,
            cancellationToken);
        if (item is null)
        {
            return;
        }

        item.Attempts++;
        var delaySeconds = Math.Min(300, Math.Pow(2, Math.Min(item.Attempts, 8)));
        item.NextAttemptAtUtc = DateTime.UtcNow.AddSeconds(delaySeconds);
        item.LastError = error.Length <= 1000 ? error : error[..1000];
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private sealed record OutboxMessage(Guid Id, string EventType, string PayloadJson);
}

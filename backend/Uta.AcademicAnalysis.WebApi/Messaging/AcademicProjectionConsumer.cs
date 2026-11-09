using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Uta.AcademicAnalysis.WebApi.Data;
using Uta.AcademicAnalysis.WebApi.Messaging;
using Uta.Middleware.Application.DTOs;

namespace Uta.AcademicAnalysis.WebApi.Messaging;

public sealed class AcademicProjectionConsumer(
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitMqSettings> settings,
    ILogger<AcademicProjectionConsumer> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions EventJsonOptions = new(JsonSerializerDefaults.Web);
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
            try
            {
                await using var connection = await factory.CreateConnectionAsync(stoppingToken);
                await using var channel = await connection.CreateChannelAsync(
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
                await channel.BasicQosAsync(0, 1, false, stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (_, delivery) =>
                {
                    try
                    {
                        var @event = JsonSerializer.Deserialize<AcademicCourseUpdatedEvent>(
                            Encoding.UTF8.GetString(delivery.Body.Span), EventJsonOptions)
                            ?? throw new JsonException("El evento recibido está vacío.");
                        await ApplyEventAsync(@event, stoppingToken);
                        await channel.BasicAckAsync(delivery.DeliveryTag, multiple: false, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        await channel.BasicNackAsync(delivery.DeliveryTag, multiple: false, requeue: true, stoppingToken);
                        throw;
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "No se pudo proyectar un evento académico; se reencolará.");
                        await channel.BasicNackAsync(delivery.DeliveryTag, multiple: false, requeue: true, stoppingToken);
                    }
                };

                await channel.BasicConsumeAsync(
                    _settings.Queue,
                    autoAck: false,
                    consumer,
                    stoppingToken);
                await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "El consumidor de Análisis perdió RabbitMQ; reintentará la conexión.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ApplyEventAsync(
        AcademicCourseUpdatedEvent @event,
        CancellationToken cancellationToken)
    {
        if (@event.EventId == Guid.Empty
            || @event.EventType != "academic.course.updated.v1"
            || @event.Snapshot.MoodleCourseId <= 0)
        {
            throw new JsonException("El evento académico no cumple el contrato esperado.");
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        if (await dbContext.ProcessedEvents.AnyAsync(
                item => item.EventId == @event.EventId, cancellationToken))
        {
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        var course = await dbContext.Courses.SingleOrDefaultAsync(
            item => item.MoodleCourseId == @event.Snapshot.MoodleCourseId,
            cancellationToken);
        if (course is null)
        {
            course = new CourseProjectionRecord
            {
                MoodleCourseId = @event.Snapshot.MoodleCourseId
            };
            dbContext.Courses.Add(course);
        }

        if (course.UpdatedAtUtc <= @event.Snapshot.UpdatedAtUtc)
        {
            course.CourseName = @event.Snapshot.CourseName;
            course.SnapshotJson = JsonSerializer.Serialize(@event.Snapshot, EventJsonOptions);
            course.UpdatedAtUtc = @event.Snapshot.UpdatedAtUtc;
            course.SourceEventId = @event.EventId;
        }

        dbContext.ProcessedEvents.Add(new ProcessedEventRecord
        {
            EventId = @event.EventId,
            EventType = @event.EventType,
            ProcessedAtUtc = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}

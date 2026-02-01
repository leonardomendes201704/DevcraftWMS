using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace PerfectApiTemplate.DemoMvc.Infrastructure.Telemetry;

public sealed class ClientTelemetryQueue
{
    private readonly Channel<ClientTelemetryEvent> _channel;
    private readonly ILogger<ClientTelemetryQueue> _logger;
    private long _dropped;

    public ClientTelemetryQueue(int capacity, ILogger<ClientTelemetryQueue> logger)
    {
        _logger = logger;
        var options = new BoundedChannelOptions(capacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropWrite
        };
        _channel = Channel.CreateBounded<ClientTelemetryEvent>(options);
    }

    public ChannelReader<ClientTelemetryEvent> Reader => _channel.Reader;

    public void Enqueue(ClientTelemetryEvent item)
    {
        if (!_channel.Writer.TryWrite(item))
        {
            var dropped = Interlocked.Increment(ref _dropped);
            if (dropped % 50 == 0)
            {
                _logger.LogWarning("Telemetry queue is full. Dropped {DroppedCount} events.", dropped);
            }
        }
    }
}

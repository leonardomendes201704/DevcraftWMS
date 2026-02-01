using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace PerfectApiTemplate.Infrastructure.Persistence.Logging.Queues;

public sealed class LogQueue<T>
{
    private readonly Channel<T> _channel;
    private readonly ILogger _logger;
    private long _dropped;
    private readonly string _name;

    public LogQueue(int capacity, ILogger logger, string name)
    {
        _channel = Channel.CreateBounded<T>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true,
            SingleWriter = false
        });
        _logger = logger;
        _name = name;
    }

    public ChannelReader<T> Reader => _channel.Reader;

    public void Enqueue(T item)
    {
        if (!_channel.Writer.TryWrite(item))
        {
            var dropped = Interlocked.Increment(ref _dropped);
            _logger.LogWarning("Log queue {QueueName} dropped items. TotalDropped={Dropped}", _name, dropped);
        }
    }
}


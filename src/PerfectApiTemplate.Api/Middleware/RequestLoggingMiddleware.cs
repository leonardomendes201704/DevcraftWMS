using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PerfectApiTemplate.Application.Abstractions.Logging;
using PerfectApiTemplate.Application.Common.Logging;
using Serilog.Context;

namespace PerfectApiTemplate.Api.Middleware;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRequestLogWriter _writer;
    private readonly LoggingOptions _options;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        IRequestLogWriter writer,
        IOptions<LoggingOptions> options,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _writer = writer;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.Requests.Enabled || IsExcludedPath(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var startedAt = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        string? requestBody = null;
        bool requestTruncated = false;
        long? requestOriginalLength = context.Request.ContentLength;
        if (IsSafeContentType(context.Request.ContentType, _options.Requests.ExcludedContentTypes))
        {
            (requestBody, requestTruncated, requestOriginalLength) =
                await ReadRequestBodyAsync(context.Request, _options.Requests.MaxBodyBytes);
            requestBody = LogMasking.SanitizeJson(requestBody, _options.Mask.JsonKeys, _options.Mask.JsonPaths, _options.Mask.Enabled);
        }

        var originalBodyStream = context.Response.Body;
        var captureStream = new ResponseCaptureStream(originalBodyStream, _options.Requests.MaxBodyBytes);
        context.Response.Body = captureStream;

        var correlationContext = context.RequestServices.GetRequiredService<ICorrelationContext>();
        try
        {
            using (LogContext.PushProperty("CorrelationId", correlationContext.CorrelationId ?? string.Empty))
            using (LogContext.PushProperty("RequestId", correlationContext.RequestId ?? string.Empty))
            {
                await _next(context);
            }
        }
        finally
        {
            stopwatch.Stop();
            var finishedAt = DateTime.UtcNow;
            string? responseBodyText = null;
            bool responseTruncated = false;
            long? responseOriginalLength = null;
            if (IsSafeContentType(context.Response.ContentType, _options.Requests.ExcludedContentTypes))
            {
                responseBodyText = captureStream.GetBodyAsString();
                responseTruncated = captureStream.IsTruncated;
                responseOriginalLength = captureStream.OriginalLength;
                responseBodyText = LogMasking.SanitizeJson(responseBodyText, _options.Mask.JsonKeys, _options.Mask.JsonPaths, _options.Mask.Enabled);
            }
            else
            {
                responseOriginalLength = captureStream.OriginalLength;
            }

            context.Response.Body = originalBodyStream;

            var statusCode = context.Response.StatusCode;
            var shouldLog = statusCode >= 500 || ShouldSample(context, correlationContext);
            if (shouldLog)
            {
                var requestHeaders = SanitizeHeaders(context.Request.Headers);
                var responseHeaders = SanitizeHeaders(context.Response.Headers);

                var entry = new RequestLogEntry(
                    startedAt,
                    finishedAt,
                    stopwatch.ElapsedMilliseconds,
                    context.Request.Method,
                    context.Request.Scheme,
                    context.Request.Host.HasValue ? context.Request.Host.Value : string.Empty,
                    context.Request.Path,
                    context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
                    statusCode,
                    context.Request.ContentType,
                    context.Request.ContentLength,
                    context.Response.ContentType,
                    context.Response.ContentLength,
                    requestHeaders,
                    responseHeaders,
                    requestBody,
                    responseBodyText,
                    requestTruncated,
                    responseTruncated,
                    requestOriginalLength,
                    responseOriginalLength,
                    context.Connection.RemoteIpAddress?.ToString(),
                    context.Request.Headers.UserAgent.ToString(),
                    correlationContext.UserId,
                    correlationContext.TenantId,
                    correlationContext.CorrelationId,
                    correlationContext.RequestId,
                    correlationContext.TraceId,
                    correlationContext.SpanId);

                try
                {
                    _writer.Enqueue(entry);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to enqueue request log");
                }
            }
        }
    }

    private bool IsExcludedPath(PathString path)
    {
        foreach (var excluded in _options.Requests.ExcludedPaths)
        {
            if (path.StartsWithSegments(excluded, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private bool ShouldSample(HttpContext context, ICorrelationContext correlationContext)
    {
        if (_options.Requests.SamplingPercent >= 100)
        {
            return true;
        }

        if (_options.Requests.SamplingPercent <= 0)
        {
            return false;
        }

        var key = correlationContext.RequestId ?? correlationContext.CorrelationId ?? context.TraceIdentifier;
        var hash = key.GetHashCode();
        var normalized = Math.Abs(hash % 100);
        return normalized < _options.Requests.SamplingPercent;
    }

    private string? SanitizeHeaders(IHeaderDictionary headers)
    {
        var allowList = _options.Requests.AllowedHeaders.Length > 0
            ? _options.Requests.AllowedHeaders
            : _options.Mask.HeaderAllowList;
        var denyList = _options.Mask.HeaderDenyList
            .Concat(_options.Requests.ExcludedHeaders)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var pairs = headers.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString()));
        return LogMasking.SanitizeHeaders(pairs, allowList, denyList);
    }

    private static bool IsSafeContentType(string? contentType, string[] excluded)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        if (excluded.Any(x => contentType.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        return contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase)
               || contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<(string? body, bool truncated, long? originalLength)> ReadRequestBodyAsync(HttpRequest request, int maxBytes)
    {
        request.EnableBuffering();
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var buffer = new char[maxBytes + 1];
        var read = await reader.ReadBlockAsync(buffer, 0, buffer.Length);
        request.Body.Position = 0;

        if (read <= maxBytes)
        {
            return (new string(buffer, 0, read), false, read);
        }

        var truncated = new string(buffer, 0, maxBytes);
        return (truncated, true, read);
    }

    private sealed class ResponseCaptureStream : Stream
    {
        private readonly Stream _inner;
        private readonly MemoryStream _buffer = new();
        private readonly long _maxBytes;
        private long _written;

        public ResponseCaptureStream(Stream inner, long maxBytes)
        {
            _inner = inner;
            _maxBytes = maxBytes;
        }

        public bool IsTruncated => _written > _maxBytes;
        public long OriginalLength => _written;

        public string? GetBodyAsString()
        {
            if (_buffer.Length == 0)
            {
                return null;
            }

            return Encoding.UTF8.GetString(_buffer.ToArray());
        }

        public override void Flush() => _inner.Flush();
        public override Task FlushAsync(CancellationToken cancellationToken) => _inner.FlushAsync(cancellationToken);

        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);

        public override void SetLength(long value) => _inner.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            Capture(buffer, offset, count);
            _inner.Write(buffer, offset, count);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Capture(buffer, offset, count);
            await _inner.WriteAsync(buffer.AsMemory(offset, count), cancellationToken);
        }

        private void Capture(byte[] buffer, int offset, int count)
        {
            _written += count;
            if (_buffer.Length >= _maxBytes)
            {
                return;
            }

            var remaining = (int)Math.Min(_maxBytes - _buffer.Length, count);
            if (remaining > 0)
            {
                _buffer.Write(buffer, offset, remaining);
            }
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;
        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }
    }
}

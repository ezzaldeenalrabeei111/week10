namespace RequestLoggerAPI.Models;

public class RequestLogEntry
{
    public DateTime Timestamp { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public long DurationMs { get; set; }
    public string UserAgent { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}

public class RequestLoggerOptions
{
    public string LogFilePath { get; set; } = "logs/api-requests.log";
    public bool EnableRequestBody { get; set; } = false;
    public int SlowRequestThresholdMs { get; set; } = 500;
}

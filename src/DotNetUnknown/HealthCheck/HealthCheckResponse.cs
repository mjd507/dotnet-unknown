namespace DotNetUnknown.HealthCheck;

public sealed record HealthCheckResponse(
    string Status,
    IReadOnlyDictionary<string, HealthCheckEntry> Results
);

public sealed record HealthCheckEntry(
    string Status,
    string? Description,
    long DurationMilliseconds,
    string? Exception,
    IReadOnlyDictionary<string, string?> Data
);
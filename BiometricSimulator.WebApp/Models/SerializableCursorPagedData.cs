namespace BiometricSimulator.WebApp.Models;

public record CursorPagedData<TData, TCursor>
    where TCursor : struct
{
    public static CursorPagedData<TData, TCursor> CreateEmpty() => new()
    {
        Payload = Array.Empty<TData>(),
        NextCursor = null,
        PreviousCursor = null
    };

    public required ICollection<TData> Payload { get; init; }
    public required TCursor? NextCursor { get; init; }
    public required TCursor? PreviousCursor { get; init; }
    public bool HasNext => NextCursor != null;
    public bool HasPrevious => PreviousCursor != null;
}

public record CursorPagedData<TData>
{
    public static CursorPagedData<TData> CreateEmpty() => new()
    {
        Payload = Array.Empty<TData>(),
        NextCursor = null,
        PreviousCursor = null,
        IsEncoded = false
    };

    public required ICollection<TData> Payload { get; init; }
    public required string? NextCursor { get; init; }
    public required string? PreviousCursor { get; init; }
    public required bool IsEncoded { get; init; }
    public bool HasNext => PreviousCursor != null;
}
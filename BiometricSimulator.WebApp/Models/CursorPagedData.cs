namespace BiometricSimulator.WebApp.Models;

public record CursorPagedData<TData, TCursor>(ICollection<TData> Payload, TCursor? LastSeen)
    where TCursor : struct
{
    public bool HasMore => LastSeen != null;
    public static CursorPagedData<TData, TCursor> CreateEmpty() => new([], null);

    public void Deconstruct(out ICollection<TData> payload, out TCursor? lastSeen, out bool hasMore)
    {
        payload = Payload;
        lastSeen = LastSeen;
        hasMore = HasMore;
    }
}

public record CursorPagedData<TData>(ICollection<TData> Payload, string? LastSeen)
{
    public bool HasMore => !string.IsNullOrEmpty(LastSeen);
    public static CursorPagedData<TData> CreateEmpty() => new([], null);

    public void Deconstruct(out ICollection<TData> payload, out string? lastSeen, out bool hasMore)
    {
        payload = Payload;
        lastSeen = LastSeen;
        hasMore = HasMore;
    }
}
using System.Threading.Channels;

namespace Api.Workers;

//Luka Kanjir
public sealed class TriggerControl : ITriggerControl
{
    private static readonly TimeSpan ttl = TimeSpan.FromMinutes(2);

    private readonly Channel<bool> signals = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
    {
        SingleReader = true,
        SingleWriter = false,
        FullMode = BoundedChannelFullMode.DropOldest
    });

    private DateTime? lastSeen;

    public bool Enabled => lastSeen.HasValue && (DateTime.UtcNow - lastSeen.Value) < ttl;

    public void Start()
    {
        lastSeen = DateTime.UtcNow;
        signals.Writer.TryWrite(true);
    }

    public void Heartbeat()
    {
        lastSeen = DateTime.UtcNow;
    }

    public void Kick() => signals.Writer.TryWrite(true);

    public async Task WaitForSignalAsync(CancellationToken ct) => await signals.Reader.ReadAsync(ct);
}
namespace Api.Workers;

//Luka Kanjir
public interface ITriggerControl
{
    bool Enabled { get; }
    void Start();
    void Heartbeat();
    void Kick();
    Task WaitForSignalAsync(CancellationToken ct);
}
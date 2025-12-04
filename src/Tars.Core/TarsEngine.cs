
namespace Tars.Core;

/// <summary>
/// The engine behind Tars for scheduling and running jobs.
/// </summary>
public static class TarsEngine
{
    private static readonly JobQueue _queue = new();
    private static readonly JobWorker _worker;
    private static readonly CancellationTokenSource _cts = new();

    static TarsEngine()
    {
        _worker = new JobWorker(_queue);
        _ = _worker.StartAsync(_cts.Token);
    }

    /// <summary>
    /// Schedule an immediate job (synchronous).
    /// </summary>
    public static void Run(Action syncWork)
    {
        var job = new Job(syncWork, DateTime.UtcNow);
        _queue.Enqueue(job);
    }

    /// <summary>
    /// Schedule an immediate job (async).
    /// </summary>
    public static void Run(Func<Task> asyncWork)
    {
        var job = new Job(asyncWork, DateTime.UtcNow);
        _queue.Enqueue(job);
    }

    /// <summary>
    /// Schedule a job (synchronous).
    /// </summary>
    public static ScheduleBuilder Schedule(Action syncWork) => new(syncWork, _queue);
    /// <summary>
    /// Schedule a job (async).
    /// </summary>
    public static ScheduleBuilder Schedule(Func<Task> asyncWork) => new(asyncWork, _queue);
    /// <summary>
    /// Stop the scheduler gracefully.
    /// </summary>
    public static void Stop() => _cts.Cancel();
}

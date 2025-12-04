namespace Tars.Core;

/// <summary>
/// Represents a Job
/// </summary>
public class Job
{
    public Guid ID { get; set; }
    public Action? SyncWork { get; set; }
    public Func<Task>? AsyncWork { get; set; }
    public DateTime ExecuteAt { get; set; }
    public int RetryCount { get; set; }
    public int MaxRetries { get; set; }

    public Job(Action work, DateTime executeAt, int maxRetries = 0)
    {
        ID = Guid.NewGuid();
        SyncWork = work;
        ExecuteAt = executeAt;
        MaxRetries = maxRetries;
    }

    public Job(Func<Task> asyncWork, DateTime executeAt, int maxRetries = 0)
    {
        ID = Guid.NewGuid();
        AsyncWork = asyncWork;
        ExecuteAt = executeAt;
        MaxRetries = maxRetries;
    }
}

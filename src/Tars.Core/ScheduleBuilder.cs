namespace Tars.Core;

/// <summary>
/// Fluent API for scheduling jobs.
/// </summary>
public class ScheduleBuilder
{
    private readonly Action? _syncWork;
    private readonly Func<Task>? _asyncWork;
    private readonly JobQueue _jobQueue;

    private int _maxRetries = 0;

    /// <summary>
    /// Initializes a new <see cref="ScheduleBuilder"/> for a synchronous job.
    /// </summary>
    /// <param name="syncWork">The work to execute.</param>
    /// <param name="jobQueue">The job queue used to enqueue the constructed job.</param>
    internal ScheduleBuilder(Action syncWork, JobQueue jobQueue)
    {
        _syncWork = syncWork;
        _jobQueue = jobQueue;
    }

    /// <summary>
    /// Initializes a new <see cref="ScheduleBuilder"/> for an asynchronous job.
    /// </summary>
    /// <param name="asyncWork">The asynchronous work to execute.</param>
    /// <param name="jobQueue">The job queue used to enqueue the constructed job.</param>
    internal ScheduleBuilder(Func<Task> asyncWork, JobQueue jobQueue)
    {
        _asyncWork = asyncWork;
        _jobQueue = jobQueue;
    }

    /// <summary>
    /// Schedules the job to be executed after the specified delay.
    /// </summary>
    /// <param name="delay">How long to wait before executing the job.</param>
    /// <returns>The unique ID of the scheduled job.</returns>
    /// <exception cref="Exception">Thrown if no work delegate was provided.</exception>
    public Guid After(TimeSpan delay)
    {
        var dateOfExecution = DateTime.UtcNow.Add(delay);

        var job = _syncWork != null ? new Job(_syncWork, DateTime.UtcNow.Add(delay), _maxRetries)
                                    : new Job(_asyncWork!, DateTime.UtcNow.Add(delay), _maxRetries);
        if (job == null)
        {
            throw new Exception("No work set");
        }

        _jobQueue.Enqueue(job);

        return job.ID;
    }

    /// <summary>
    /// Schedules the job to execute at a specific UTC time.
    /// </summary>
    /// <param name="dateOfExecution">The exact UTC timestamp when the job should run.</param>
    /// <returns>The unique ID of the scheduled job.</returns>
    public Guid At(DateTime dateOfExecution)
    {
        var job = _syncWork != null ? new Job(_syncWork, dateOfExecution, _maxRetries)
                                    : new Job(_asyncWork!, dateOfExecution, _maxRetries);

        _jobQueue.Enqueue(job);

        return job.ID;
    }

    /// <summary>
    /// Specifies the maximum number of retry attempts if the job fails.
    /// </summary>
    /// <param name="maxRetries">The number of retry attempts allowed.</param>
    /// <returns>The same <see cref="ScheduleBuilder"/> instance for fluent chaining.</returns>
    /// <remarks>
    /// Retry delays follow an exponential back-off pattern: 2^n seconds for the n-th retry (1, 2, 4, 8...).
    /// </remarks>
    public ScheduleBuilder WithRetry(int maxRetries)
    {
        _maxRetries = maxRetries;
        return this;
    }
}

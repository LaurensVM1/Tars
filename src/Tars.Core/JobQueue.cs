namespace Tars.Core;

/// <summary>
/// Thread-safe job queue using PriorityQueue.
/// </summary>
public class JobQueue
{
    private readonly PriorityQueue<Job, DateTime> _jobs = new();
    private readonly HashSet<Guid> _removed = [];
    private readonly object _lock = new();

    public void Enqueue(Job job)
    {
        lock (_lock)
        {
            _jobs.Enqueue(job, job.ExecuteAt);
        }
    }

    public Job? GetNextDueJob()
    {
        lock (_lock)
        {
            while (_jobs.TryPeek(out var job, out var at))
            {
                // If job is canceled, skip it
                if (_removed.Contains(job.ID))
                {
                    _jobs.Dequeue();
                    continue;
                }

                if (at <= DateTime.UtcNow)
                {
                    _jobs.Dequeue();
                    return job;
                }

                break;
            }
        }

        return null;
    }

    public void Remove(Guid jobID)
    {
        lock (_lock)
        {
            _removed.Add(jobID);
        }
    }
}

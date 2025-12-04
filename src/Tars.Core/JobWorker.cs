using Microsoft.Extensions.Hosting;

namespace Tars.Core;

/// <summary>
/// Background worker to execute jobs.
/// </summary>
internal class JobWorker : IHostedService
{
    private readonly JobQueue _queue;
    public JobWorker(JobQueue queue)
    {
        _queue = queue;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var job = _queue.GetNextDueJob();

            if (job != null)
            {
                ExecuteJob(job);
            }

            await Task.Delay(100, cancellationToken);
        }
    }

    private void ExecuteJob(Job job)
    {
        Task.Run(async () =>
        {
            try
            {
                if (job.SyncWork != null) job.SyncWork();
                if (job.AsyncWork != null) await job.AsyncWork();

                Console.WriteLine($"Job {job.ID} completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Job {job.ID} failed: {ex.Message}");

                if (job.RetryCount < job.MaxRetries)
                {
                    job.RetryCount++;

                    var retryDelay = TimeSpan.FromSeconds(Math.Pow(2, job.RetryCount));
                    Job? retryJob = null;

                    if (job.SyncWork != null)
                    {
                        retryJob = new Job(job.SyncWork, DateTime.UtcNow.Add(retryDelay), job.MaxRetries)
                        {
                            RetryCount = job.RetryCount
                        };
                    }

                    if (job.AsyncWork != null) 
                    {
                        retryJob = new Job(job.AsyncWork, DateTime.UtcNow.Add(retryDelay), job.MaxRetries)
                        {
                            RetryCount = job.RetryCount
                        };
                    }

                    _queue.Enqueue(retryJob!);
                    Console.WriteLine($"Job {job.ID} scheduled for retry in {retryDelay.TotalSeconds}s");
                }
                else
                {
                    Console.WriteLine($"Job {job.ID} failed permanently after {job.RetryCount + 1} attempts");
                }
            }
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("TARS worker stopping...");
        return Task.CompletedTask;
    }
}

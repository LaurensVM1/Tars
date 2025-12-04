namespace Tars.Core.Tests;

public class JobTests
{
    [Fact]
    public void Job_SyncConstructor_InitializesCorrectly()
    {
        var executeAt = DateTime.UtcNow.AddMinutes(5);
        var maxRetries = 3;

        var job = new Job(() => Console.WriteLine("Hello world"), executeAt, maxRetries);

        Assert.NotEqual(Guid.Empty, job.ID);
        Assert.NotNull(job.SyncWork);
        Assert.Null(job.AsyncWork);
        Assert.Equal(executeAt, job.ExecuteAt);
        Assert.Equal(maxRetries, job.MaxRetries);
        Assert.Equal(0, job.RetryCount);
    }

    [Fact]
    public void Job_AsyncConstructor_InitializesCorrectly()
    {
        var executeAt = DateTime.UtcNow.AddMinutes(5);
        var maxRetries = 2;

        var job = new Job(async () => await Task.CompletedTask, executeAt, maxRetries);

        Assert.NotEqual(Guid.Empty, job.ID);
        Assert.Null(job.SyncWork);
        Assert.NotNull(job.AsyncWork);
        Assert.Equal(executeAt, job.ExecuteAt);
        Assert.Equal(maxRetries, job.MaxRetries);
        Assert.Equal(0, job.RetryCount);
    }

    [Fact]
    public void Job_GeneratesUniqueIDs()
    {
        var job1 = new Job(() => { }, DateTime.UtcNow);
        var job2 = new Job(() => { }, DateTime.UtcNow);

        Assert.NotEqual(job1.ID, job2.ID);
    }
}
namespace Tars.Core.Tests;

public class JobQueueTests
{
    [Fact]
    public void Enqueue_AddsJobToQueue()
    {
        var queue = new JobQueue();
        var job = new Job(() => { }, DateTime.UtcNow.AddSeconds(-1));

        queue.Enqueue(job);
        var retrieved = queue.GetNextDueJob();

        Assert.NotNull(retrieved);
        Assert.Equal(job.ID, retrieved.ID);
    }

    [Fact]
    public void GetNextDueJob_ReturnsNull_WhenQueueEmpty()
    {
        var queue = new JobQueue();

        var job = queue.GetNextDueJob();

        Assert.Null(job);
    }

    [Fact]
    public void GetNextDueJob_ReturnsNull_WhenJobNotDue()
    {
        var queue = new JobQueue();
        var job = new Job(() => { }, DateTime.UtcNow.AddHours(1));

        queue.Enqueue(job);
        var retrieved = queue.GetNextDueJob();

        Assert.Null(retrieved);
    }
}

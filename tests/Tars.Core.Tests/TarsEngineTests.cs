namespace Tars.Core.Tests;

public class TarsEngineTests
{
    [Fact]
    public async Task Run_ExecutesSyncJobImmediately()
    {
        var executed = false;

        TarsEngine.Run(() => executed = true);
        await Task.Delay(500);

        Assert.True(executed);
    }

    [Fact]
    public async Task Run_ExecutesAsyncJobImmediately()
    {
        var executed = false;

        TarsEngine.Run(async () => { await Task.Delay(10); executed = true; });
        await Task.Delay(500);

        Assert.True(executed);
    }

    [Fact]
    public void Schedule_ReturnsScheduleBuilder()
    {
        var builder = TarsEngine.Schedule(() => { });

        Assert.NotNull(builder);
        Assert.IsType<ScheduleBuilder>(builder);
    }

    [Fact]
    public void Schedule_ReturnsScheduleBuilderForAsync()
    {
        var builder = TarsEngine.Schedule(async () => await Task.CompletedTask);

        Assert.NotNull(builder);
        Assert.IsType<ScheduleBuilder>(builder);
    }
}

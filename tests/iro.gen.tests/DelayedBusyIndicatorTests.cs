using System.Collections.Concurrent;
using System.Diagnostics;
using IroGen;

namespace IroGenTests;

public class DelayedBusyIndicatorTests
{
    [Fact]
    public async Task FastWorkNeverShowsIndicator()
    {
        var changes = new ConcurrentQueue<bool>();
        Assert.Equal(42, await DelayedBusyIndicator.RunAsync(async () => { await Task.Delay(20); return 42; }, changes.Enqueue));
        await Task.Delay(1100); // A completed run must not leave a delayed appearance behind.
        Assert.Empty(changes);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SlowWorkShowsAfterOneSecondAndAlwaysHides(bool fail)
    {
        var changes = new ConcurrentQueue<bool>();
        var work = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var appeared = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var elapsed = Stopwatch.StartNew();
        var run = DelayedBusyIndicator.RunAsync(() => work.Task, visible =>
        {
            changes.Enqueue(visible);
            if (visible) appeared.TrySetResult();
        });
        Assert.False(run.IsCompleted);
        Assert.Empty(changes);
        await appeared.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.True(elapsed.Elapsed >= TimeSpan.FromMilliseconds(950));
        if (fail)
        {
            work.SetException(new InvalidOperationException("Testfehler"));
            await Assert.ThrowsAsync<InvalidOperationException>(() => run);
        }
        else { work.SetResult(42); Assert.Equal(42, await run); }
        Assert.Equal(new[] { true, false }, changes.ToArray());
    }

    [Fact]
    public async Task EarlyCancellationDoesNotShowIndicator()
    {
        using var cancellation = new CancellationTokenSource();
        var changes = new ConcurrentQueue<bool>();
        var run = DelayedBusyIndicator.RunAsync(async () =>
        {
            await Task.Delay(10000, cancellation.Token);
            return 42;
        }, changes.Enqueue, cancellation.Token);
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => run);
        Assert.Empty(changes);
    }
}

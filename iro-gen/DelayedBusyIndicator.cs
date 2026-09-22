namespace IroGen;

/// <summary>Shows activity only while work is still pending after one second.</summary>
public static class DelayedBusyIndicator
{
    public static async Task<T> RunAsync<T>(Func<Task<T>> work, Action<bool> show, CancellationToken token = default)
    {
        using var delayCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
        bool visible = false;
        try
        {
            var task = work();
            var delay = Task.Delay(TimeSpan.FromSeconds(1), delayCancellation.Token);
            if (await Task.WhenAny(task, delay) == delay && !task.IsCompleted && !token.IsCancellationRequested)
            {
                visible = true;
                show(true);
            }
            return await task;
        }
        finally
        {
            delayCancellation.Cancel();
            if (visible) show(false);
        }
    }
}

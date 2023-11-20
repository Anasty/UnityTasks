using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncTimerExtension
{
    public CancellationTokenSource cancellationTokenSource = default;

    public event Action OnCurrentTimeChanged = delegate { };

    public event Action OnTimerEnd = delegate { };

    public TimeSpan CurrentTime { get; private set; } = default;

    private TimeSpan timeStep = TimeSpan.FromSeconds(1);

    /// <summary>
    /// запуск таймера
    /// </summary>
    /// <param name="yieldInstruction"></param>
    public void StartTimer(bool needRestart = true)
    {
        timeStep = TimeSpan.FromSeconds(1);
        if (needRestart)
        {
            CurrentTime = TimeSpan.FromSeconds(0);
        }
        StartTimerAsync();
    }

    /// <summary>
    /// Запуск обратного отсчета
    /// </summary>
    /// <param name="time"></param>
    /// <param name="yieldInstruction"></param>
    public void StartCountdown(TimeSpan time)
    {
        timeStep = TimeSpan.FromSeconds(-1);
        CurrentTime = time;
        StartTimerAsync();
    }

    public void StopTimer()
    {
        cancellationTokenSource.Cancel();
    }

    private async void StartTimerAsync()
    {
        cancellationTokenSource = new CancellationTokenSource();

        try
        {
            while (Application.isPlaying)
            {
                await Task.Delay(1000, cancellationTokenSource.Token);
                CurrentTime += timeStep;
                OnCurrentTimeChanged();
                if (CurrentTime == TimeSpan.Zero)
                {
                    OnTimerEnd();
                }
                Debug.Log(CurrentTime);
            }
        }
        catch
        {
            Debug.Log("Task was cancelled");
        }
        finally
        {
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

    }
}

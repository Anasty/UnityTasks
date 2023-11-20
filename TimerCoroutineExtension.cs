using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerCoroutineExtension
{
    public event Action OnCurrentTimeChanged = delegate { };

    public event Action OnTimerEnd = delegate { };

    public TimeSpan CurrentTime { get; private set; } = default;

    private TimeSpan timeStep = TimeSpan.FromSeconds(1);
    private MonoBehaviour owner;

    private CoroutineExtension coroutine = default;

    private YieldInstruction _yieldInstruction = new WaitForSeconds(1f);

    private IEnumerator Timer()
    {
        while (owner.isActiveAndEnabled)
        {
            yield return _yieldInstruction;
            CurrentTime += timeStep;
            OnCurrentTimeChanged();
            if (CurrentTime == TimeSpan.Zero)
            {
                OnTimerEnd();
            }
            Debug.Log(CurrentTime);
        }
    }

    /// <summary>
    /// запуск таймера
    /// </summary>
    /// <param name="yieldInstruction"></param>
    public void StartTimer(YieldInstruction yieldInstruction, bool needRestart = true)
    {
        timeStep = TimeSpan.FromSeconds(1);
        _yieldInstruction = yieldInstruction;
        if (needRestart)
        {
            CurrentTime = TimeSpan.FromSeconds(0);
        }

        coroutine.Coroutine = owner.StartCoroutine(Timer());
    }

    /// <summary>
    /// Запуск обратного отсчета
    /// </summary>
    /// <param name="time"></param>
    /// <param name="yieldInstruction"></param>
    public void StartCountdown(TimeSpan time, YieldInstruction yieldInstruction)
    {
        timeStep = TimeSpan.FromSeconds(-1);
        _yieldInstruction = yieldInstruction;
        CurrentTime = time;

        coroutine.Coroutine = owner.StartCoroutine(Timer());
    }

    public void StopTimer()
    {
        coroutine.Coroutine = null;
    }

    public TimerCoroutineExtension(MonoBehaviour owner)
    {
        coroutine = new CoroutineExtension(owner);
        this.owner = owner;
    }
}

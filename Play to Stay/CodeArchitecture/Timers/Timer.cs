using System;
using UnityEngine;

namespace GeneralLogic.Timer
{
    [Serializable]
    public class Timer
    {
        private float timerCurrent = 0f;
        public float TimerCurrent { get { return timerCurrent; } }
        public float timerSetting = 1f;

        public event Action OnTimerFinished;

        private bool stopped = false;

        public void SetTo(float _timerSetting)
        {
            timerSetting = _timerSetting;
        }

        public void Randomize(float min, float max)
        {
            timerSetting = UnityEngine.Random.Range(min, max);
        }

        public void Reset()
        {
            timerCurrent = 0f;
        }

        public void Stop()
        {
            Reset();
            stopped = true;
        }

        public void Pause()
        {
            stopped = true;
        }

        public void Start()
        {
            stopped = false;
        }

        public void UpdateTimer()
        {
            if (stopped == false)
            {
                timerCurrent += Time.deltaTime;
                if (timerCurrent >= timerSetting)
                {
                    timerCurrent = 0f;
                    OnTimerFinished?.Invoke();
                }
            }
        }

        public Timer(float _timerSetting)
        {
            timerSetting = _timerSetting;
        }
    }
}
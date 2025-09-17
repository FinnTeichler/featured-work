using System;

namespace GeneralLogic.Timer
{
    [Serializable]
    public class TimerRandomized : Timer
    {
        public float min;
        public float max;

        public TimerRandomized(float _timerSetting, float _randomRangeMin, float _randomRangeMax) : base(_timerSetting)
        {
            min = _randomRangeMin;
            max = _randomRangeMax;
            Initialize();
        }

        public void Initialize()
        {
            base.Randomize(min, max);
        }
    }
}
using System;
using UnityEngine;


namespace FinnTeichler.TimeSystem
{
    public class TimeTickSystem : MonoBehaviour
    {
        public class OnTickEventArgs : EventArgs
        {
            public int tick;
        }

        public static event EventHandler<OnTickEventArgs> OnTick;

        public enum TimeMeasurement
        {
            Tick,
            RealSecond,
            RealMinute,
            RealHour,
            RealDay
        }

        private const float TICK_TIMER_MAX = 0.2f;

        private int tick;
        private float tickTimer;

        void Start()
        {
            tick = 0;
        }

        void Update()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= TICK_TIMER_MAX)
            {
                tickTimer -= TICK_TIMER_MAX;
                tick++;

                OnTick?.Invoke(this, new OnTickEventArgs { tick = tick });
            }
        }

        public static float ConvertTimeMeasurement(float value, TimeMeasurement from, TimeMeasurement to)
        {
            float ticksPerRealSecond = 1 / TICK_TIMER_MAX;
            float ticksPerRealMinute = ticksPerRealSecond * 60;
            float ticksPerRealHour = ticksPerRealMinute * 60;
            float ticksPerRealDay = ticksPerRealHour * 24;

            switch (from)
            {
                case TimeMeasurement.Tick:
                    switch (to)
                    {
                        case TimeMeasurement.Tick:
                            return value;
                        case TimeMeasurement.RealSecond:
                            return value / ticksPerRealSecond;
                        case TimeMeasurement.RealMinute:
                            return value / ticksPerRealMinute;
                        case TimeMeasurement.RealHour:
                            return value / ticksPerRealHour;
                        case TimeMeasurement.RealDay:
                            return value / ticksPerRealDay;
                    }

                    break;

                case TimeMeasurement.RealSecond:
                    switch (to)
                    {
                        case TimeMeasurement.Tick:
                            return value * ticksPerRealSecond;
                        case TimeMeasurement.RealSecond:
                            return value;
                        case TimeMeasurement.RealMinute:
                            return value / 60;
                        case TimeMeasurement.RealHour:
                            return value / 60 / 60;
                        case TimeMeasurement.RealDay:
                            return value / 60 / 60 / 24;
                    }

                    break;

                case TimeMeasurement.RealMinute:
                    switch (to)
                    {
                        case TimeMeasurement.Tick:
                            return value * ticksPerRealMinute;
                        case TimeMeasurement.RealSecond:
                            return value * 60;
                        case TimeMeasurement.RealMinute:
                            return value;
                        case TimeMeasurement.RealHour:
                            return value / 60;
                        case TimeMeasurement.RealDay:
                            return value / 60 / 24;
                    }

                    break;

                case TimeMeasurement.RealHour:
                    switch (to)
                    {
                        case TimeMeasurement.Tick:
                            return value * ticksPerRealHour;
                        case TimeMeasurement.RealSecond:
                            return value * 60 * 60;
                        case TimeMeasurement.RealMinute:
                            return value * 60;
                        case TimeMeasurement.RealHour:
                            return value;
                        case TimeMeasurement.RealDay:
                            return value / 24;
                    }

                    break;

                case TimeMeasurement.RealDay:
                    switch (to)
                    {
                        case TimeMeasurement.Tick:
                            return value * ticksPerRealDay;
                        case TimeMeasurement.RealSecond:
                            return value * 60 * 60 * 24;
                        case TimeMeasurement.RealMinute:
                            return value * 60 * 24;
                        case TimeMeasurement.RealHour:
                            return value * 24;
                        case TimeMeasurement.RealDay:
                            return value;
                    }

                    break;
            }

            Debug.LogWarning("TimeTickSystem TimeMeasurement Conversion seems to be missing a case: From " +
                             from.ToString() + " to " + to.ToString() +
                             ". No Conversion possible, returned same value.");
            return value;
        }
    }
}
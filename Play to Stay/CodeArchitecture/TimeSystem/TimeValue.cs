using System;
using UnityEngine;


namespace FinnTeichler.TimeSystem
{
    [Serializable]
    public class TimeValue
    {
        [SerializeField] private int value;
        [SerializeField] private TimeTickSystem.TimeMeasurement measurement;

        public int GetValueInTicks()
        {
            return (int)TimeTickSystem.ConvertTimeMeasurement(value, measurement, TimeTickSystem.TimeMeasurement.Tick);
        }

        public float GetValueIn(TimeTickSystem.TimeMeasurement demandedMeasurement)
        {
            return TimeTickSystem.ConvertTimeMeasurement(value, measurement, demandedMeasurement);
            ;
        }

        public TimeValue(int _value, TimeTickSystem.TimeMeasurement _measurement)
        {
            value = _value;
            measurement = _measurement;
        }
    }
}
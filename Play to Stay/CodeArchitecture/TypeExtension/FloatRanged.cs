namespace GeneralLogic.TypeUtility
{
    [System.Serializable]
    public struct FloatRanged
    {
        public float min;
        public float max;

        public FloatRanged(float _min, float _max)
        {
            min = _min;
            max = _max;
        }
    }
}
using System;
using UnityEngine;
//using FinnTeichler.TimeSystem;

namespace SpiritGarden.Creature.Interaction.Data
{
    [Serializable]
    public class LikeInfo
    {
        public InteractionProperty[] InteractionProperties { get => interactionProperties; }
        [SerializeField] private InteractionProperty[] interactionProperties;

        public LikeLevel LikeLevel { get => likeLevel; }
        [SerializeField] private LikeLevel likeLevel;

        //public int Repeats { get => repeats; }
        //[SerializeField] private int repeats = 1;

        //public TimeValue Cooldown { get => cooldown; }
        //[SerializeField] private TimeValue cooldown = new(1, TimeTickSystem.TimeMeasurement.RealSecond);
    }
}
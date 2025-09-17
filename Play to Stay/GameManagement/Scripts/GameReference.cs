using SpiritGarden.Character;
using UnityEngine;

namespace SpiritGarden
{
    public class GameReference : MonoBehaviour
    {

        public CharacterReference RefCharacter { get { return refCharacter; } }
        [SerializeField] private CharacterReference refCharacter;

        public UIReference RefUI { get { return refUI; } }
        [SerializeField] private UIReference refUI;

        public GameObject ItemPickupTemplate { get { return itemPickupTemplate; } }
        [SerializeField] private GameObject itemPickupTemplate;

        public GamePause GamePause { get { return gamePause; } }
        [SerializeField] private GamePause gamePause;

        public TimeMeasurement TimeMeasurement { get { return timeMeasurement; } }
        [SerializeField] private TimeMeasurement timeMeasurement;

        public Tutorial Tutorial { get { return tutorial; } }
        [SerializeField] private Tutorial tutorial;


        private void Awake()
        {
            IfNullLogError(refCharacter);
            IfNullLogError(refUI);
            IfNullLogError(gamePause);
            IfNullLogError(timeMeasurement);
            IfNullLogError(tutorial);

            refCharacter.refGame = this;
            refUI.refGame = this;
            tutorial.refGame = this;
        }

        private void IfNullLogError(Component component)
        {
            if (component == null)
            {
                Debug.LogError($"{this} is missing a {component} component.");
            }
        }
    }
}
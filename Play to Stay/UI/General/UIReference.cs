using UnityEngine;
using FinnTeichler.UI;
using SpiritGarden.InventoryExtension;
using Opsive.UltimateInventorySystem.UI.Panels;

namespace SpiritGarden
{
    public class UIReference : MonoBehaviour
    {
        [HideInInspector] public GameReference refGame;

        //canvas
        public Canvas CanvasFloatingUI { get { return canvasFloatingUI; } }
        [SerializeField] private Canvas canvasFloatingUI;
        public Canvas CanvasInstructions { get { return canvasInstructions; } }
        [SerializeField] private Canvas canvasInstructions;
        public Canvas CanvasIntro { get { return canvasIntro; } }
        [SerializeField] private Canvas canvasIntro;
        public Canvas CanvasInventory { get { return canvasInventory; } }
        [SerializeField] private Canvas canvasInventory;
        public Canvas CanvasGameMenu { get { return canvasGameMenu; } }
        [SerializeField] private Canvas canvasGameMenu;
        public Canvas CanvasTutorial { get { return canvasTutorial; } }
        [SerializeField] private Canvas canvasTutorial;

        //menu controls
        //public InventoryMenuControls InventoryMenuControls { get {  return inventoryMenuControls; } }
        //[SerializeField] private InventoryMenuControls inventoryMenuControls;

        public ReadMeMenu GameInstructionsMenu { get { return gameInstructionsMenu; } }
        [SerializeField] private ReadMeMenu gameInstructionsMenu;

        public ReadMeMenu GameIntroMenu { get { return gameIntroMenu; } }
        [SerializeField] private ReadMeMenu gameIntroMenu;

        public GameMenu GameQuitMenu { get { return gameQuitMenu; } }
        [SerializeField] private GameMenu gameQuitMenu;

        public DisplayPanel InventoryDisplayPanel { get { return inventoryDisplayPanel; } }
        [SerializeField] private DisplayPanel inventoryDisplayPanel;

        //floating ui
        public FloatingUI FloatingUIFocusedCreature { get { return floatingUIFocusedCreature; } }
        [Space(10)][SerializeField] private FloatingUI floatingUIFocusedCreature;

        public TimeDisplay TimeDisplay { get { return timeDisplay; } }
        [SerializeField] private TimeDisplay timeDisplay;

        public bool IntroIsClosed {  get { return introIsClosed; } }
        private bool introIsClosed;

        void Awake()
        {
            IfNullLogError(canvasFloatingUI);
            IfNullLogError(canvasInstructions);
            IfNullLogError(canvasIntro);
            IfNullLogError(canvasInventory);
            IfNullLogError(canvasGameMenu);

            //IfNullLogError(inventoryMenuControls);
            IfNullLogError(gameInstructionsMenu);
            IfNullLogError(gameIntroMenu);
            IfNullLogError(gameQuitMenu);
            IfNullLogError(inventoryDisplayPanel);

            IfNullLogError(floatingUIFocusedCreature);
            IfNullLogError(timeDisplay);

            //inventoryMenuControls.refUI = this;
            gameInstructionsMenu.refUI = this;
            gameIntroMenu.refUI = this;
            gameQuitMenu.refUI = this;
            timeDisplay.refUI = this;
        }

        private void OnEnable()
        {
            GameIntroMenu.OnClose += StartTutorial;
        }

        private void OnDisable()
        {
            GameIntroMenu.OnClose -= StartTutorial;
        }

        private void StartTutorial()
        {
            refGame.Tutorial.enabled = true;
            introIsClosed = true;
        }

        private void IfNullLogError(Component component)
        {
            if (component == null)
            {
                Debug.LogError($"{this} is missing a {component} component.");
            }
        }

        public void SetMenu(Canvas canvas, bool open)
        {
            canvas.transform.GetChild(0).gameObject.SetActive(open);
            
            if (open)
            {
                CanvasFloatingUI.gameObject.SetActive(false);
                refGame.GamePause.Pause();
            }
            else
            {
                CanvasFloatingUI.gameObject.SetActive(true);
                refGame.GamePause.Resume();
            }
        }
    }
}
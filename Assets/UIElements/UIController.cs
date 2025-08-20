using UnityEngine;
using UnityEngine.UIElements;

namespace UIController
{
    public class UIController : MonoBehaviour
    {

        public GameObject uiPanel;
        [SerializeField]private PlayerAssets.PlayerInputSystem _playerInputSystem;

        public VisualElement ui;
        public Button startButton;
        public Button quitButton;

        private void Start()
        {
            ShowUI();
        }
        private void Awake()
        {
            ui = GetComponent<UIDocument>().rootVisualElement;

            // auto-assign player input system if not assigned in inspector
            if (_playerInputSystem == null)
            {
                _playerInputSystem = PlayerAssets.PlayerInputSystem.Instance;
                if (_playerInputSystem == null)
                {
                    // fallback (works on newer Unity). If you use older Unity remove this or add version guards
                    _playerInputSystem = UnityEngine.Object.FindFirstObjectByType<PlayerAssets.PlayerInputSystem>();
                }
            }

            Debug.Log($"[UIController] Awake: uiPanel={(uiPanel ? uiPanel.name : "null")}, _playerInputSystem={_playerInputSystem?.name ?? "null"}");
        }

        private void OnEnable()
        {
            startButton = ui.Q<Button>("StartButton");
            startButton.clicked += OnStartButtonClicked;
            quitButton = ui.Q<Button>("QuitButton");
            quitButton.clicked += OnQuitButtonClicked;
        }

        private void OnQuitButtonClicked()
        {
            Application.Quit();
        }

        private void OnStartButtonClicked()
        {
            HideUI();
        }

        public void ShowUI()
        {
            uiPanel.SetActive(true);
            Debug.Log("[UIController] ShowUI()");
            if (_playerInputSystem != null) _playerInputSystem.ToggleInput(false);
        }

        public void HideUI()
        {
            uiPanel.SetActive(false);
            Debug.Log("[UIController] HideUI()");
            if (_playerInputSystem != null) _playerInputSystem.ToggleInput(true);
        }
    }
}

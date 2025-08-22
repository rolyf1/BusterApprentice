using PlayerAssets; // for PlayerInputSystem
using UnityEngine;
using UnityEngine.InputSystem; // <--- added for InputAction
using UnityEngine.UIElements;

public class DialogueController : MonoBehaviour
{
    public static DialogueController Instance { get; private set; }

    [Header("UIDocument / panel")]
    [Tooltip("Assign the GameObject that contains the UIDocument for dialogue (keep it active).")]
    public GameObject dialoguePanelGameObject; // the GameObject that has a UIDocument

    private UIDocument _uiDocument;
    private VisualElement _root;

    // UI fields we expect to exist inside the UIDocument
    private Label _speakerLabel;
    private Label _contentLabel;
    private Button _nextButton;
    private Button _closeButton;
    private VisualElement _dialogueBox;
    private VisualElement _dialogue;

    // dialogue state
    private string[] _lines;
    private int _lineIndex;
    private bool _isOpen;

    // references
    private PlayerInputSystem _playerInput;

    // --- input subscription fields (new) ---
    private InputAction _interactAction;
    private float _dialogueInputCooldown = 0.18f;
    private float _lastDialogueInputTime = -1f;
    // ------------------------------------------------

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Debug.LogWarning($"Multiple DialogueController instances: {name}");

        _playerInput = PlayerInputSystem.Instance ?? FindObjectOfType<PlayerInputSystem>();

        // Try to locate UIDocument from assigned object or from this GameObject
        if (dialoguePanelGameObject != null)
        {
            // Ensure assigned object is active so UIDocument root exists
            if (!dialoguePanelGameObject.activeInHierarchy)
            {
                Debug.Log("[DialogueController] Activating dialoguePanelGameObject temporarily for initialization.");
                dialoguePanelGameObject.SetActive(true);
            }

            _uiDocument = dialoguePanelGameObject.GetComponent<UIDocument>();
            if (_uiDocument == null)
                Debug.LogError($"DialogueController: No UIDocument found on assigned GameObject '{dialoguePanelGameObject.name}'.");
        }
        else
        {
            _uiDocument = GetComponent<UIDocument>() ?? FindObjectOfType<UIDocument>();
            if (_uiDocument == null)
                Debug.LogError("DialogueController: No UIDocument assigned and none found automatically. Assign dialoguePanelGameObject in inspector.");
            else
                dialoguePanelGameObject = _uiDocument.gameObject;
        }

        if (_uiDocument != null)
        {
            _root = _uiDocument.rootVisualElement;
            if (_root == null)
                Debug.LogError("[DialogueController] rootVisualElement is null. Ensure UIDocument has a valid VisualTreeAsset.");
            else
            {
                // Hide visually via USS class at start, but keep UIDocument GameObject active.
                _root.EnableInClassList("hide", true);
                _root.style.display = DisplayStyle.None;
            }
        }
    }

    private void OnEnable()
    {
        // Bind UI elements if root is available
        if (_uiDocument == null || _root == null)
        {
            Debug.LogError("[DialogueController] Cannot bind UI elements: UIDocument/root missing. Ensure dialoguePanelGameObject is assigned and active.");
            return;
        }

        _speakerLabel = _root.Q<Label>("Speaker");
        _contentLabel = _root.Q<Label>("Content");
        _dialogueBox = _root.Q<VisualElement>("DialogueBox");
        _dialogue = _root.Q<VisualElement>("Dialogue");

        _nextButton = _root.Q<Button>("NextButton");
        _closeButton = _root.Q<Button>("CloseButton");

        if (_nextButton != null) _nextButton.clicked += OnNextClicked;
        else Debug.LogWarning("[DialogueController] NextButton not found (name must be 'NextButton').");

        if (_closeButton != null) _closeButton.clicked += OnCloseClicked;
        else Debug.LogWarning("[DialogueController] CloseButton not found (name must be 'CloseButton').");

        // Diagnostics: print what was found
        Debug.Log($"[DialogueController] Bound UI: Speaker={(_speakerLabel != null ? "OK" : "null")}, Content={(_contentLabel != null ? "OK" : "null")}, Next={(_nextButton != null ? "OK" : "null")}, Close={(_closeButton != null ? "OK" : "null")}");

        // --- subscribe to Interact InputAction (if available) ---
        TrySubscribeToInteract();
    }

    private void OnDisable()
    {
        if (_nextButton != null) _nextButton.clicked -= OnNextClicked;
        if (_closeButton != null) _closeButton.clicked -= OnCloseClicked;

        // Unsubscribe input action
        if (_interactAction != null)
        {
            _interactAction.performed -= OnInteractPerformed;
            _interactAction = null;
        }
    }

    // Public API: show a dialogue with title and array of lines
    public void ShowDialogue(string speakerName, string[] lines)
    {
        if (_isOpen) return; // already open

        if (_root == null)
        {
            Debug.LogError("[DialogueController] Cannot ShowDialogue: UI root is null. Make sure the UIDocument GameObject is active and assigned.");
            return;
        }

        _lines = lines ?? new string[] { "" };
        _lineIndex = 0;

        // Preferred: keep UIDocument active and show/hide by class and display
        _root.EnableInClassList("hide", false);        // remove hide class
        _root.style.display = DisplayStyle.Flex;      // ensure visible

        if (_speakerLabel != null) _speakerLabel.text = speakerName ?? "";
        UpdateTextLabel();

        _isOpen = true;

        // disable player input while dialogue is open
        if (_playerInput != null)
            _playerInput.ToggleInput(false);
    }

    private void UpdateTextLabel()
    {
        if (_contentLabel != null)
        {
            string t = (_lines != null && _lines.Length > 0 && _lineIndex >= 0 && _lineIndex < _lines.Length) ?
                       _lines[_lineIndex] : "";
            _contentLabel.text = t;
        }
        else
        {
            Debug.LogWarning("[DialogueController] _contentLabel is null when trying to update text.");
        }
    }

    private void OnNextClicked()
    {
        if (!_isOpen) return;

        _lineIndex++;
        if (_lines != null && _lineIndex < _lines.Length)
        {
            UpdateTextLabel();
        }
        else
        {
            CloseDialogue();
        }
    }

    private void OnCloseClicked()
    {
        CloseDialogue();
    }

    private void CloseDialogue()
    {
        _isOpen = false;

        // Prefer hiding via USS class instead of SetActive(false)
        if (_root != null)
        {
            _root.EnableInClassList("hide", true);
            _root.style.display = DisplayStyle.None;
        }

        // re-enable player input
        if (_playerInput != null)
            _playerInput.ToggleInput(true);
    }

    // Optional convenience: Show dialogue and auto clear interact or other flags
    public void ShowDialogueAndClearInput(string speakerName, string[] lines)
    {
        ShowDialogue(speakerName, lines);
        if (_playerInput != null) _playerInput.InteractInput(false);
    }

    // ------------------- Input subscription helpers -------------------
    private void TrySubscribeToInteract()
    {
        // Try to get PlayerInput component via the PlayerInputSystem singleton first, then fallback
        PlayerInput playerInputComp = null;
        if (_playerInput != null)
            playerInputComp = _playerInput.GetComponent<PlayerInput>();
        if (playerInputComp == null)
            playerInputComp = FindObjectOfType<PlayerInput>();

        if (playerInputComp == null)
        {
            Debug.LogWarning("[DialogueController] No PlayerInput found for input subscription. Controller input will not advance dialogues. Ensure a PlayerInput exists in the scene.");
            return;
        }

        if (playerInputComp.actions == null)
        {
            Debug.LogWarning("[DialogueController] Found PlayerInput but it has no actions asset assigned.");
            return;
        }

        // Look up the Interact action by name (must match your Input Action asset)
        _interactAction = playerInputComp.actions.FindAction("Interact");
        if (_interactAction == null)
        {
            Debug.LogWarning("[DialogueController] Could not find 'Interact' action on PlayerInput.actions. Ensure the action name matches.");
            return;
        }

        // Subscribe
        _interactAction.performed += OnInteractPerformed;
        Debug.Log("[DialogueController] Subscribed to Interact action for controller input.");
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        // only respond if dialogue is open
        if (!_isOpen) return;

        // basic debounce
        if (Time.time - _lastDialogueInputTime < _dialogueInputCooldown) return;
        _lastDialogueInputTime = Time.time;

        // Advance dialogue (reuse existing logic)
        OnNextClicked();

        // clear player input interact flag if present
        if (_playerInput != null) _playerInput.InteractInput(false);
    }
}

using PlayerAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class DialogueManager3D : MonoBehaviour
{
    [Header("Assign your 3D TextMeshPro component")]
    public TextMeshPro dialogueText; // NOTE: TextMeshPro (3D), not TextMeshProUGUI
    public GameObject continueIndicator; // optional, can be a small 3D arrow or sprite

    [Header("Dialogue")]
    [TextArea(3, 10)] public List<string> lines = new List<string>();
    public float typeSpeed = 0.03f;
    public PlayerInput playerInput;
    public PlayerInputSystem playerInputSystem;
    public bool inDialogue = false;

    private int index = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    [SerializeField] private float advanceCooldown = 0.3f; // seconds between advances
    private float nextAdvanceTime = 0f;

    public Transform PlayerPos;
    public int posYOffset = 10;
    public int posZOffset = -7;
    public int posXOffset = 0;
    private Vector3 dialoguePos;

    public string NPCnumber = "1";

    void Awake()
    {
        // Try to auto-find if not assigned
        if (dialogueText == null)
        {
            dialogueText = GetComponent<TextMeshPro>();
            if (dialogueText == null)
                dialogueText = GetComponentInChildren<TextMeshPro>();
        }

        if (dialogueText == null)
            Debug.LogError($"DialogueManager3D on '{gameObject.name}' has no TextMeshPro assigned. Assign the TextMeshPro component (3D) in the inspector.");
    }

    void Update()
    {
        if (Time.time >= nextAdvanceTime)
        {
            if (playerInputSystem.interact)
            {
                if (isTyping)
                    FinishTyping();
                else
                    NextLine();
            }
        }
    }
    private void LateUpdate()
    {
        dialoguePos = new Vector3(PlayerPos.position.x + posXOffset, PlayerPos.position.y + posYOffset, PlayerPos.position.z + posZOffset);
        transform.position = Vector3.Lerp(transform.position, dialoguePos, 0.1f);
    }

    public void StartDialogue(List<string> newLines, TextMeshPro targetText)
    {
        inDialogue = true;
        if (targetText == null)
        {
            Debug.LogWarning("StartDialogueWithExistingText called with null targetText.");
            return;
        }

        lines = newLines ?? new List<string>();
        index = 0;

        ShowLine();
    }

    public void StartOrAdvanceDialogue(List<string> newLines, TextMeshPro targetText)
    {
        if (dialogueText != null && inDialogue)
        {
            // Same NPC, already speaking Å® advance instead
            NextLine();
            return;
        }

        // Otherwise start fresh
        StartDialogue(newLines, dialogueText);
    }

    void ShowLine()
    {
        if (index < lines.Count)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeSentence(lines[index]));
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        if (dialogueText == null) yield break;
        isTyping = true;
        dialogueText.text = "";
        if (continueIndicator != null) continueIndicator.SetActive(false);

        foreach (char c in sentence)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed); // use WaitForSecondsRealtime if you prefer
        }

        isTyping = false;
        if (continueIndicator != null) continueIndicator.SetActive(true);
    }

    void FinishTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = lines[index];
        isTyping = false;
        if (continueIndicator != null) continueIndicator.SetActive(true);
    }

    public void NextLine()
    {
        index++;
        if (index < lines.Count) ShowLine();
        else EndDialogue();
    }

    void EndDialogue()
    {
        if (continueIndicator != null) continueIndicator.SetActive(false);
        dialogueText.text = "";
        Debug.Log("Dialogue finished");
        inDialogue = false;

    }
}

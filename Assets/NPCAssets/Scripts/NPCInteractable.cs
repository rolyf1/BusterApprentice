using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class NPCInteractable : MonoBehaviour
{
    public string NPCnumber = "1";
    [Header("Dialogue")]
    public string npcName = "NPC";
    [TextArea(3, 10)] public List<string> lines = new List<string>();

    public DialogueManager3D dialogueManager; // assign in Inspector

    public void Interact()
    {
        if (dialogueManager == null)
        {
            Debug.LogError("No DialogueManager3D assigned.");
            return;
        }

        NPCnumber = gameObject.name; // Assuming the GameObject name is the NPC number

        dialogueManager.StartOrAdvanceDialogue(lines, dialogueManager.dialogueText);
    }
}


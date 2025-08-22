using UnityEngine;
using System.Collections.Generic;

public class ShowNPC : MonoBehaviour
{
    public DialogueManager3D dialogueManager;
    public NPCInteractable NPCInteractable;
    public GameObject NPC1;
    public GameObject NPC2;
    public GameObject NPC3;
    public GameObject junkieJohnny;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (NPCInteractable.NPCnumber == "1")
        {
            NPC1.SetActive(true);
        }
        else if (NPCInteractable.NPCnumber == "2")
        {
            NPC2.SetActive(true);
        }
        else if (NPCInteractable.NPCnumber == "3")
        {
            NPC3.SetActive(true);
        }
        else if (NPCInteractable.NPCnumber == "4")
        {
            junkieJohnny.SetActive(true);
        }
    }
}

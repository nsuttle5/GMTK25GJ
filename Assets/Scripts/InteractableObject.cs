using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] dialogueLines;

    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogueLines, this.transform);
    }
}

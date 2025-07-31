using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public float textSpeed = 0.03f;

    private string[] lines;
    private int currentLine;
    private Coroutine typingCoroutine;
    private bool isActive = false;
    private Transform interactableTransform;
    private Transform playerTransform;
    public float closeDistance = 3f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        dialogueBox.SetActive(false);
    }

    public void StartDialogue(string[] newLines, Transform interactable)
    {
        lines = newLines;
        currentLine = 0;
        dialogueBox.SetActive(true);
        isActive = true;
        interactableTransform = interactable;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        ShowLine();
    }

    // Overload for backwards compatibility
    public void StartDialogue(string[] newLines)
    {
        StartDialogue(newLines, null);
    }

    void ShowLine()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(lines[currentLine]));
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void Update()
    {
        if (!isActive) return;

        // Close if player walks away from interactable
        if (interactableTransform != null && playerTransform != null)
        {
            float dist = Vector2.Distance(playerTransform.position, interactableTransform.position);
            if (dist > closeDistance)
            {
                dialogueBox.SetActive(false);
                isActive = false;
                return;
            }
        }

        if (dialogueBox.activeInHierarchy && Input.GetKeyDown(KeyCode.R))
        {
            if (typingCoroutine != null && dialogueText.text != lines[currentLine])
            {
                // Finish line instantly
                StopCoroutine(typingCoroutine);
                dialogueText.text = lines[currentLine];
            }
            else
            {
                currentLine++;
                if (currentLine < lines.Length)
                {
                    ShowLine();
                }
                else
                {
                    dialogueBox.SetActive(false);
                    isActive = false;
                }
            }
        }
    }
}

using UnityEngine;
using TMPro;

using UnityEngine.SceneManagement;
public class CreditsRolling : MonoBehaviour
{
    [Header("Credits Settings")]
    public TMP_Text creditsText; // Assign in inspector
    public float scrollSpeed = 40f; // Units per second
    [Header("Scene Transition")]
    public string nextSceneName;

    private RectTransform creditsRect;
    private float startY;
    private float endY;
    private bool finished = false;
    private bool bottomReached = false;
    private float bottomReachedTime = 0f;
    public float stopDelay = 4f; // seconds to keep rolling after bottom
    private bool readyForContinue = false;

    void Start()
    {
        if (creditsText == null)
        {
            Debug.LogError("CreditsRolling: No TMP_Text assigned!");
            enabled = false;
            return;
        }
        creditsRect = creditsText.GetComponent<RectTransform>();
        startY = creditsRect.anchoredPosition.y;
        // Calculate endY so that the bottom of the text is just above the bottom of the parent
        float parentHeight = ((RectTransform)creditsRect.parent).rect.height;
        float textHeight = creditsRect.rect.height;
        endY = startY + (textHeight + parentHeight) * 0.5f;
    }

    void Update()
    {
        if (creditsRect == null) return;
        if (!finished)
        {
            Vector2 pos = creditsRect.anchoredPosition;
            pos.y += scrollSpeed * Time.deltaTime;
            creditsRect.anchoredPosition = pos;

            float parentHeight = ((RectTransform)creditsRect.parent).rect.height;
            float textBottom = creditsRect.anchoredPosition.y - creditsRect.rect.height;
            if (!bottomReached && textBottom > parentHeight * 0.5f)
            {
                bottomReached = true;
                bottomReachedTime = Time.time;
            }
            if (bottomReached && (Time.time - bottomReachedTime) >= stopDelay)
            {
                finished = true;
                readyForContinue = true;
            }
        }
        else if (readyForContinue)
        {
            // Wait for E key to continue
            if (Input.GetKeyDown(KeyCode.E) && !string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CreditsDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI gameCompletedText;
    public TextMeshProUGUI creditsScrollingText;
    public TextMeshProUGUI nextGameTeaser;
    public TextMeshProUGUI pressAnyKeyText;

    [Header("Credits Settings")]
    public float scrollSpeed = 100f;  // Reduced from 200f for better readability

    [Header("Name Generation")]
    public string[] firstNames = {
        "Alex", "Jordan", "Casey", "Morgan", "Taylor", "Sam", "Chris", "Riley", "Avery", "Blake",
        "Quinn", "Reese", "Sage", "Robin", "Emery", "Hayden", "Payton", "Skyler", "River", "Finley",
        "Rowan", "Phoenix", "Dakota", "Kai", "Nova", "Sage", "Wren", "Ash", "Lane", "Remy"
    };

    public string[] lastNames = {
        "Anderson", "Thompson", "Williams", "Martinez", "Garcia", "Rodriguez", "Chen", "Kim", "Patel",
        "Johnson", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Jackson", "White", "Harris",
        "Martin", "Clark", "Lewis", "Walker", "Hall", "Young", "King", "Wright", "Green", "Baker",
        "Adams", "Nelson", "Carter", "Mitchell", "Perez", "Roberts", "Turner", "Phillips", "Campbell"
    };

    public string[] gameRoles = {
        "Executive Producer", "Creative Director", "Lead Game Designer", "Senior Programmer",
        "Art Director", "Lead Artist", "Senior Environment Artist", "Character Artist",
        "UI/UX Designer", "Lead Animator", "Technical Artist", "Sound Designer",
        "Audio Director", "Music Composer", "Lead Level Designer", "Gameplay Programmer",
        "Graphics Programmer", "Engine Programmer", "Quality Assurance Lead", "Senior QA Tester",
        "Community Manager", "Marketing Director", "Producer", "Associate Producer",
        "Game Writer", "Narrative Designer", "Localization Manager", "Build Engineer",
        "DevOps Engineer", "Technical Director", "Studio Manager", "Finance Director"
    };

    private RectTransform creditsRect;
    private string generatedCredits;
    private bool creditsCompleted = false;

    void Start()
    {
        SetupCredits();
        GenerateCredits();
        StartCoroutine(ScrollCredits());

        // Show "Press any key" at all times
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.text = "Press any key to continue...";
            pressAnyKeyText.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // Allow manual skip at any time
        if (Input.anyKeyDown)
        {
            GoToNextGame();
        }

        // Blink the "Press Any Key" text at all times
        if (pressAnyKeyText != null && pressAnyKeyText.gameObject.activeInHierarchy)
        {
            float alpha = (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f;
            Color color = pressAnyKeyText.color;
            color.a = alpha;
            pressAnyKeyText.color = color;
        }
    }

    void SetupCredits()
    {
        if (GameManager.Instance == null) return;

        GameEntry completedGame = GameManager.Instance.CurrentGame;

        // Game completed text
        if (gameCompletedText != null)
        {
            gameCompletedText.text = $"{completedGame.gameTitle}\nCOMPLETED!";
        }

        // Next game teaser
        if (nextGameTeaser != null)
        {
            if (GameManager.Instance.CurrentGameNumber < GameManager.Instance.TotalGames)
            {
                nextGameTeaser.text = "COMING NEXT: Even MORE of the same magic!";
            }
            else
            {
                nextGameTeaser.text = "THE FRANCHISE LIVES FOREVER...";
            }
        }

        // Setup scrolling text
        if (creditsScrollingText != null)
        {
            creditsRect = creditsScrollingText.GetComponent<RectTransform>();
        }
    }

    void GenerateCredits()
    {
        System.Text.StringBuilder credits = new System.Text.StringBuilder();
        var currentGame = GameManager.Instance.CurrentGame;
        string company = currentGame.productionCompany;
        int roles = currentGame.numberOfRoles;

        // Studio header
        credits.AppendLine($"<size=150%><color=#FFD700>{company}</color></size>");
        credits.AppendLine("<size=120%>Presents</size>");
        credits.AppendLine("");
        credits.AppendLine("");

        if (GameManager.Instance != null)
        {
            credits.AppendLine($"<size=140%><color=#87CEEB>{GameManager.Instance.CurrentGame.gameTitle}</color></size>");
        }

        credits.AppendLine("");
        credits.AppendLine("");
        credits.AppendLine("<size=120%><color=#FFD700>CREDITS</color></size>");
        credits.AppendLine("");

        // Generate random roles
        List<string> usedRoles = new List<string>();
        System.Random random = new System.Random();

        for (int i = 0; i < roles && i < gameRoles.Length; i++)
        {
            string role;
            do
            {
                role = gameRoles[random.Next(gameRoles.Length)];
            } while (usedRoles.Contains(role));

            usedRoles.Add(role);

            string firstName = firstNames[random.Next(firstNames.Length)];
            string lastName = lastNames[random.Next(lastNames.Length)];

            credits.AppendLine($"<color=#FFFFFF>{role}</color>");
            credits.AppendLine($"<color=#B0B0B0>{firstName} {lastName}</color>");
            credits.AppendLine("");
        }

        // Studio footer
        credits.AppendLine("");
        credits.AppendLine($"<size=120%><color=#FFD700>A {company} Production</color></size>");
        credits.AppendLine("");
        credits.AppendLine("<color=#808080>In memory of all the talented developers</color>");
        credits.AppendLine("<color=#808080>whose studios were lost but whose</color>");
        credits.AppendLine("<color=#808080>passion lives on in every game.</color>");
        credits.AppendLine("");
        credits.AppendLine("");
        credits.AppendLine("<size=80%><color=#606060>Thank you for playing</color></size>");
        credits.AppendLine("");
        credits.AppendLine("");
        credits.AppendLine("");
        credits.AppendLine("");

        generatedCredits = credits.ToString();

        if (creditsScrollingText != null)
        {
            creditsScrollingText.text = generatedCredits;

            // Debug the credits
            Debug.Log($"Generated credits text length: {generatedCredits.Length}");
            Debug.Log($"Credits text color: {creditsScrollingText.color}");
            Debug.Log($"Credits text active: {creditsScrollingText.gameObject.activeInHierarchy}");
            Debug.Log($"Credits RectTransform size: {creditsScrollingText.rectTransform.sizeDelta}");
            Debug.Log($"Credits RectTransform position: {creditsScrollingText.rectTransform.anchoredPosition}");

            // Ensure text is visible
            if (creditsScrollingText.color.a < 0.5f)
            {
                Color textColor = creditsScrollingText.color;
                textColor.a = 1f;
                creditsScrollingText.color = textColor;
                Debug.Log("Credits text alpha was too low, setting to full opacity");
            }
        }
    }

    IEnumerator ScrollCredits()
    {
        if (creditsRect == null)
        {
            Debug.LogError("Credits RectTransform is null!");
            yield break;
        }

        // Wait one frame to ensure text layout is calculated
        yield return null;

        // Force the text to recalculate its preferred height
        creditsScrollingText.ForceMeshUpdate();

        // Start credits at the bottom of the screen (visible immediately)
        Vector2 startPos = creditsRect.anchoredPosition;
        startPos.y = -100f;  // Start slightly below screen
        creditsRect.anchoredPosition = startPos;

        Debug.Log($"Starting credits scroll from Y: {startPos.y}, Screen height: {Screen.height}");

        // Use per-game scroll distance from GameEntry
        float endY = 12000f;
        if (GameManager.Instance != null && GameManager.Instance.CurrentGame != null)
        {
            endY = GameManager.Instance.CurrentGame.creditsScrollDistance;
        }
        Debug.Log($"Text preferred height: {creditsScrollingText.preferredHeight}");
        Debug.Log($"Using per-game end Y: {endY}");

        float currentY = startPos.y;

        while (currentY < endY)
        {
            currentY += scrollSpeed * Time.deltaTime;
            Vector2 newPos = creditsRect.anchoredPosition;
            newPos.y = currentY;
            creditsRect.anchoredPosition = newPos;

            // Log progress every 100 units
            if (Mathf.FloorToInt(currentY / 100f) != Mathf.FloorToInt((currentY - scrollSpeed * Time.deltaTime) / 100f))
            {
                Debug.Log($"Credits Y position: {currentY:F0} / {endY:F0}");
            }

            yield return null;
        }

        Debug.Log("Credits scroll completed");
    }

    void GoToNextGame()
    {
        StopAllCoroutines(); // Stop any remaining coroutines
        GameManager.Instance?.GoToNextGame();
    }
}
